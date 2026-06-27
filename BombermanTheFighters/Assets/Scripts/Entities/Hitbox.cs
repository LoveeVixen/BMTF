// LOVEEVIXEN
using InputSystem;
using UnityEngine;
using System.Collections.Generic;

namespace EntitySystem
{
    public class Hitbox : MonoBehaviour
    {
        [SerializeField] string hitboxName = "Limb";
        public enum HitboxType { high, middle, low };
        [SerializeField] HitboxType hitboxType = HitboxType.middle;

        private bool attackOnCollision = false;
        private bool hasHit = false;
        private BoxCollider col;
        private GameObject display;
        private MeshRenderer displayRender;
        private Entity entity;
        private Attack performingAttack;

        private void Awake()
        {
            col = GetComponent<BoxCollider>();
            entity = GetComponentInParent<Entity>();

            // Setup debug display.
            display = Instantiate(entity.GetHitboxDisplayPrefab(), transform);
            displayRender = display.GetComponent<MeshRenderer>();
            displayRender.material = entity.GetNormalMaterial();
            display.transform.localPosition = col.center;
            display.transform.localScale = col.size;
            ShowHitboxDisplay(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            // Check that the hitbox has collided into another hitbox belonging to a different entity.
            Hitbox otherHitbox = other.gameObject.GetComponent<Hitbox>();
            if (otherHitbox != null && otherHitbox.GetEntity() != entity && !hasHit)
            {
                // Hit target hitbox.
                if (attackOnCollision)
                {
                    // Let all hitboxes belonging to this entity know it has hit a target.
                    foreach (Hitbox relatedHitbox in entity.GetHitboxesList())
                        relatedHitbox.HasHitTarget();

                    // Calculate direction hit target will stumble towards after being hit.
                    Player player = otherHitbox.GetEntity() as Player;
                    Vector3 stumbleDir = new Vector3(-otherHitbox.GetEntity().transform.forward.x, 0f, -otherHitbox.GetEntity().transform.forward.z);
                    if (player != null)
                        stumbleDir = new Vector3(entity.transform.forward.x, 0f, entity.transform.forward.z);

                    // Register hit into session manager.
                    SessionManager.instance.AddRegisteredHit(otherHitbox, performingAttack, stumbleDir);
                }
            }
        }

        public void AttackOnCollision(bool toggle)
        {
            if(toggle)
            {
                attackOnCollision = true;
                displayRender.material = entity.GetAttackMaterial();
            }
            else
            {
                attackOnCollision = false;
                displayRender.material = entity.GetNormalMaterial();

                // Reset attack references.
                hasHit = false;
                performingAttack = null;
            }
        }

        public void ShowHitboxDisplay(bool displayHitbox)
        {
            display.SetActive(displayHitbox);
        }

        public Entity GetEntity() {  return entity; }
        public string GetHitboxName() { return hitboxName; }
        public HitboxType GetHitboxType() { return hitboxType; }
        public void SetPerformingAttack(Attack setPerformingAttack) { performingAttack = setPerformingAttack; }
        public void HasHitTarget() { hasHit = true; }
    }
}