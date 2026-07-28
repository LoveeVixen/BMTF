// LOVEEVIXEN
using Audio;
using InputSystem;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EntitySystem
{
    public struct HitData
    {
        public string hitboxID;
        public float damage;
        public float stumbleSpeed;
        public float yVelocityLaunch;
        public float stumbleTime;
        public int attackType; // Look for struct 'AttackType' in the Attack script file.
        public Vector3 stumbleDirection;
    }

    public class Entity : MonoBehaviourPunCallbacks, IPunObservable
    {
        private Health health;

        [Header("Entity Hitbox")]
        [SerializeField] GameObject hitboxDisplayPrefab;
        [SerializeField] Material normalMaterial;
        [SerializeField] Material attackMaterial;
        private List<EntityHitbox> hitboxes = new List<EntityHitbox>();

        // Entity physics.
        private const float gravity = 9.81f;
        [SerializeField] bool effectedByGravity = true;
        private float yVel;
        private bool airborne;

        private void Awake()
        {
            health = GetComponent<Health>();
            SetupCharacterHitbox();
            OnAwake();
        }

        private void Update()
        {
            OnTick();
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.IsWriting)
            {
                stream.SendNext(health.CurrentHealth);
            }
            else if(stream.IsReading)
            {
                health.CurrentHealth = (float)stream.ReceiveNext();
            }
        }

        public virtual void OnAwake() { }
        public virtual void Output() { }
        public virtual void OnTick()
        {
            // Check that the entity is airborne.
            if(transform.position.y > 0f)
                airborne = true;

            // Apply movement on Y axis.
            if(photonView.IsMine)
                transform.position += (transform.up * yVel) * Time.deltaTime;

            if (airborne)
            {
                // Apply gravity.
                if(effectedByGravity && photonView.IsMine) yVel -= (gravity * 15f) * Time.deltaTime;

                // Ground player once they reach the floor.
                if (transform.position.y <= 0f)
                {
                    if (photonView.IsMine)
                    {
                        yVel = 0f;
                        Vector3 snapToGround = new Vector3(transform.position.x, 0f, transform.position.z);
                        transform.position = snapToGround;
                        airborne = false;
                    }

                    OnLand();
                }
            }
        }

        public virtual void OnLand() { }

        public void MoveTo(Vector3 setPosition)
        {
            transform.position = setPosition;
        }

        // Round entity's position to be by 1 decimal place.
        public void SnapPosition()
        {
            float x = Mathf.Round(transform.position.x * 10f) * 0.1f;
            float y = Mathf.Round(transform.position.y * 10f) * 0.1f;
            float z = Mathf.Round(transform.position.z * 10f) * 0.1f;

            transform.position = new Vector3(x, y, z);
        }

        public void SetupCharacterHitbox()
        {
            EntityHitbox[] hb = GetComponentsInChildren<EntityHitbox>();
            foreach (EntityHitbox hitbox in hb)
                hitboxes.Add(hitbox);
        }

        public Vector3 Pos()
        {
            return transform.position;
        }

        public void SetYVelocity(float setYVel) { yVel = setYVel; }

        public EntityHitbox FindHitbox(string hitboxName)
        {
            foreach(EntityHitbox hitbox in hitboxes)
            {
                if(hitbox.GetHitboxName() == hitboxName)
                    return hitbox;
            }

            print("Failed to find hitbox with name: " + hitboxName);
            return null;
        }

        public void RegisterHit(EntityHitbox otherHitbox, Attack attack, Vector3 stumbleDirection)
        {
            string hitboxID = otherHitbox.HitboxID();
            float damage = attack.damage;
            float stumbleSpeed = attack.stumbleSpeed;
            float yVel = attack.yVelocityLaunch;
            float stumbleTime = attack.stumbleTime;
            int type = (int)attack.attackType;
            float stumbleX = stumbleDirection.x;
            float stumbleY = stumbleDirection.y;
            float stumbleZ = stumbleDirection.z;

            photonView.RPC("RPC_RegisterHit", RpcTarget.All, hitboxID, damage, stumbleSpeed, yVel, stumbleTime, type, stumbleX, stumbleY, stumbleZ);
        }

        [PunRPC]
        public void RPC_RegisterHit(string hitboxID, float damage, float stumbleSpeed, float yVel, float stumbleTime, int type, float stumbleX, float stumbleY, float stumbleZ)
        {
            HitData hitData = new HitData
            {
                hitboxID = hitboxID,
                damage = damage,
                stumbleSpeed = stumbleSpeed,
                yVelocityLaunch = yVel,
                stumbleTime = stumbleTime,
                attackType = type,
                stumbleDirection = new Vector3(stumbleX, stumbleY, stumbleZ)
            };

            SessionManager.instance.AddRegisteredHit(hitData);
        }

        public Health GetHealth() { return health; }
        public GameObject GetHitboxDisplayPrefab() { return hitboxDisplayPrefab; }
        public List<EntityHitbox> GetHitboxesList() {  return hitboxes; }

        public Material GetNormalMaterial() { return normalMaterial; }
        public Material GetAttackMaterial() { return attackMaterial; }
        public float GetGravity() {  return gravity; }
        public bool IsAirborne() { return airborne; }
        public bool EffectedByGravity { get { return effectedByGravity; } set { effectedByGravity = value; } }
    }
}