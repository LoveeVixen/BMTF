// LOVEEVIXEN
using Audio;
using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

namespace EntitySystem
{
    public class Explosion : Entity
    {
        [SerializeField] float lastTime = 1f;
        private Animator anim;
        private bool disappearing;
        private List<Health> hit = new List<Health>();

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

        private void OnTriggerEnter(Collider other)
        {
            if (photonView.IsMine)
            {
                // Check that the hitbox has collided into another hitbox belonging to a different entity.
                EntityHitbox otherHitbox = other.gameObject.GetComponent<EntityHitbox>();
                if (otherHitbox != null && !hit.Contains(otherHitbox.GetEntity().GetHealth()))
                {
                    // Calculate direction hit target will stumble towards after being hit.
                    Player player = otherHitbox.GetEntity() as Player;
                    Vector3 stumbleDir = new Vector3(-otherHitbox.GetEntity().transform.forward.x, 0f, -otherHitbox.GetEntity().transform.forward.z);
                    if (player != null)
                        stumbleDir = new Vector3(transform.forward.x, 0f, transform.forward.z);

                    // Register hit into session manager.
                    HitData hitData = new HitData();
                    hitData.hitboxID = otherHitbox.HitboxID();
                    hitData.damage = 60f;
                    hitData.stumbleSpeed = 0f;
                    hitData.yVelocityLaunch = 60f;
                    hitData.stumbleTime = 0.02f;
                    hitData.attackType = 1;
                    hitData.stumbleDirection = stumbleDir;

                    RegisterHit(hitData, stumbleDir);

                    // Add entity to hit list to make sure it cannot be hurt again by same explosion.
                    hit.Add(otherHitbox.GetEntity().GetHealth());
                }
            }
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