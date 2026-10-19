using UnityEngine;
using Photon.Pun;

namespace EntitySystem
{
    public class GrabInteraction : MonoBehaviourPunCallbacks
    {
        [SerializeField] float grabDistance = 2.0f;
        private Transform grabPoint;
        private Player player;
        private Health health;
        private Player defense;
        private int defenseID = -1;

        private void Awake()
        {
            player = GetComponent<Player>();
            health = GetComponent<Health>();
        }

        public void Grab(Player other, string animationName)
        {
            photonView.RPC("RPC_Grab", RpcTarget.All, other.photonView.ViewID, player.GetExecuteAnimationOnGrab());
        }

        [PunRPC]
        void RPC_Grab(int pvId, string animationName)
        {
            PhotonView pv = PhotonView.Find(pvId);
            if (pv != null)
            {
                defense = pv.gameObject.GetComponent<Player>();
                Animator defenseAnim = defense.GetAnim();

                // Change camera angle.
                SessionManager.instance.GetCam().FocusOnPlayer(player);

                // Set grabbed opponent's parent to the grab point, to control it's transform.
                defense.transform.SetParent(grabPoint);
                defense.Captive();
                defense.transform.localPosition = Vector3.zero;
                defense.transform.localRotation = Quaternion.identity;

                // Transfer defense's player ownership temporarily to the one grabbing.
                defenseID = pv.OwnerActorNr;
                pv.TransferOwnership(photonView.Owner);
                defense.PlayAnimation(animationName + "_Captive");
            }
        }

        public Player FindPlayerInFront()
        {
            RaycastHit[] hit = Physics.RaycastAll(transform.position, transform.forward, grabDistance);
            for(int i = 0; i < hit.Length; i++)
            {
                Player otherPlayer = hit[i].collider.GetComponentInParent<Player>();
                if (otherPlayer != player)
                    return otherPlayer;
            }

            return null;
        }

        public void ReleaseCaptive()
        {
            if(defense != null && photonView.IsMine) photonView.RPC("RPC_ReleaseCaptive", RpcTarget.All, defense.photonView.ViewID);
        }

        [PunRPC]
        void RPC_ReleaseCaptive(int pvId)
        {
            PhotonView pv = PhotonView.Find(pvId);
            if (pv != null)
            {
                defense = pv.gameObject.GetComponent<Player>();
                defense.transform.SetParent(null);
                Vector3 rot = defense.transform.localRotation.eulerAngles;
                defense.transform.localRotation = Quaternion.Euler(0f, rot.y, 0f);
                defense.Lay();
                defense.EffectedByGravity = true;
                defense = null;

                // Change camera angle.
                SessionManager.instance.GetCam().Normal();

                // Return ownership of player to original client.
                pv.TransferOwnership(PhotonNetwork.CurrentRoom.GetPlayer(defenseID));
            }
        }

        public void CameraFlip()
        {
            if (photonView.IsMine) photonView.RPC("RPC_CameraFlip", RpcTarget.All);
        }

        [PunRPC]
        void RPC_CameraFlip()
        {
            SessionManager.instance.FlipCamera = !SessionManager.instance.FlipCamera;
        }

        // For use in animation events. Take damage at certain point of captive throw animation.
        public void RemoveHealth(float damage)
        {
            if (photonView.IsMine) health.RemoveHealth(damage);
        }

        public Transform GrabPoint {  get { return grabPoint; } set { grabPoint = value; } }
    }
}