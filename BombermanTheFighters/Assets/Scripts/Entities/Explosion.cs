// LOVEEVIXEN
using Audio;
using Photon.Pun;
using UnityEngine;

namespace EntitySystem
{
    public class Explosion : Entity
    {
        [SerializeField] float lastTime = 1f;
        private Animator anim;
        private bool disappearing;

        public override void OnAwake()
        {
            base.OnAwake();
            anim = GetComponent<Animator>();
        }

        private void Start()
        {
            SoundProperties properties = new SoundProperties();
            properties.minDistance = 30f;
            AudioManager.instance.PlaySound("Explosion", Pos(), properties);
        }

        public override void OnTick()
        {
            base.OnTick();
            if(lastTime > 0f)
            {
                lastTime -= Time.deltaTime;
                if(lastTime < 0f)
                    lastTime = 0f;
            }

            if (lastTime == 0f && !disappearing)
            {
                disappearing = true;
                anim.Play("Disappear");
            }
        }

        public void Destroy()
        {
            if(photonView.IsMine)
                PhotonNetwork.Destroy(gameObject);
        }
    }
}