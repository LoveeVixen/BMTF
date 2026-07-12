// LOVEEVIXEN
using Fusion;
using InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Analytics.IAnalytic;

namespace EntitySystem
{
    public struct NetworkHitData : INetworkStruct
    {
        public int hitboxID;
        public float stumbleSpeed;
        public float yVelocityLaunch;
        public float stumbleTime;
        public int attackType; // Look for struct 'AttackType' in the Attack script file.
        public Vector3 stumbleDirection;
    }

    public class Entity : NetworkBehaviour
    {
        private NetworkTransform networkTransform;
        private NetworkRunner runner;

        [Header("Entity Hitbox")]
        [SerializeField] GameObject hitboxDisplayPrefab;
        [SerializeField] Material normalMaterial;
        [SerializeField] Material attackMaterial;
        private List<EntityHitbox> hitboxes = new List<EntityHitbox>();

        // Entity physics.
        private const float gravity = 9.81f;
        private float yVel;
        private bool airborne;

        private void Awake()
        {
            networkTransform = GetComponent<NetworkTransform>();
            runner = NetworkManager.instance.GetRunner();
            SetupCharacterHitbox();
            OnAwake();
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            OnTick();
        }

        public virtual void OnAwake() { }
        public virtual void OnTick()
        {
            // Check that the entity is airborne.
            if(transform.position.y > 0f)
                airborne = true;

            // Apply movement on Y axis.
            networkTransform.Teleport(transform.position + (transform.up * yVel));
            if (airborne)
            {
                // Apply gravity.
                yVel -= gravity / 200f;

                // Ground player once they reach the floor.
                if (transform.position.y <= 0f)
                {
                    yVel = 0f;
                    Vector3 snapToGround = new Vector3(transform.position.x, 0f, transform.position.z);
                    networkTransform.Teleport(snapToGround);
                    airborne = false;
                    OnLand();
                }
            }
        }

        public virtual void OnLand() { }

        // Round entity's position to be by 1 decimal place.
        public void SnapPosition()
        {
            float x = Mathf.Round(transform.position.x * 10f) * 0.1f;
            float y = Mathf.Round(transform.position.y * 10f) * 0.1f;
            float z = Mathf.Round(transform.position.z * 10f) * 0.1f;
            networkTransform.Teleport(new Vector3(x, y, z));
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
            int hitboxID = otherHitbox.HitboxID();
            float attackStumbleSpeed = attack.stumbleSpeed;
            float attackYVelLaunch = attack.yVelocityLaunch;
            float attackStumbleTime = attack.stumbleTime;
            int attackType = (int)attack.attackType;
            float attackStumbleDirX = stumbleDirection.x;
            float attackStumbleDirY = stumbleDirection.y;
            float attackStumbleDirZ = stumbleDirection.z;

            NetworkHitData hitData = new NetworkHitData
            {
                hitboxID = hitboxID,
                stumbleSpeed = attackStumbleSpeed,
                yVelocityLaunch = attackYVelLaunch,
                stumbleTime = attackStumbleTime,
                attackType = attackType,
                stumbleDirection = new Vector3(attackStumbleDirX, attackStumbleDirY, attackStumbleDirZ)
            };

            RPC_RegisterHit(hitData);
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_RegisterHit(NetworkHitData hitData)
        {
            SessionManager.instance.AddRegisteredHit(hitData);
        }

        public NetworkTransform NetworkTransform() { return networkTransform; }
        public NetworkRunner GetRunner() { return runner; }
        public GameObject GetHitboxDisplayPrefab() { return hitboxDisplayPrefab; }
        public List<EntityHitbox> GetHitboxesList() {  return hitboxes; }

        public Material GetNormalMaterial() { return normalMaterial; }
        public Material GetAttackMaterial() { return attackMaterial; }
        public float GetGravity() {  return gravity; }
        public bool IsAirborne() { return airborne; }
    }
}