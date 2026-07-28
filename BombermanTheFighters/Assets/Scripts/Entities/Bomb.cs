// LOVEEVIXEN
using UnityEngine;
using Photon.Pun;

namespace EntitySystem
{
    public class Bomb : Entity
    {
        private bool pauseFuseTime = true;
        [SerializeField] float fuseTime = 3f;
        [SerializeField] float explosionSize = 14f;
        private bool exploded = false;
        private bool explodeOnCollision = false;
        private bool defused = false;
        private Animator anim;

        public override void OnAwake()
        {
            base.OnAwake();
            anim = GetComponent<Animator>();
        }

        public override void OnTick()
        {
            base.OnTick();
            if(photonView.IsMine)
            {
                if(fuseTime > 0f && !pauseFuseTime)
                {
                    fuseTime -= Time.deltaTime;
                    if(fuseTime < 0f)
                        fuseTime = 0f;
                }

                if(fuseTime == 0f && !exploded)
                    Explode();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(photonView.IsMine)
            {
                Explosion explosion = other.gameObject.GetComponentInParent<Explosion>();
                if (explosion != null && !exploded)
                    Explode();
            }
        }

        private void OnDestroy()
        {
            if (photonView.IsMine && !defused)
            {
                GameObject projectile = PhotonNetwork.Instantiate("Projectiles/Explosion", Pos(), Quaternion.identity);
                projectile.transform.localScale = new Vector3(explosionSize, explosionSize, explosionSize);
            }
        }

        public void Explode()
        {
            if (photonView.IsMine && !exploded)
            {
                exploded = true;
                photonView.RPC("RPC_Explode", RpcTarget.All);
            }
        }

        [PunRPC]
        void RPC_Explode()
        {
            anim.Play("Explode");
        }

        public void Destroy()
        {
            if (photonView.IsMine)
                PhotonNetwork.Destroy(gameObject);
        }

        public bool PauseFuseTime {  get { return pauseFuseTime; } set { pauseFuseTime = value; } }
        public bool ExplodeOnCollision {  get { return explodeOnCollision; } set { explodeOnCollision = value; } }
    }
}