// LOVEEVIXEN
using System.Collections.Generic;
using UnityEngine;

namespace EntitySystem
{
    public class Entity : MonoBehaviour
    {
        [Header("Entity Hitbox")]
        [SerializeField] GameObject hitboxDisplayPrefab;
        [SerializeField] Material normalMaterial;
        [SerializeField] Material attackMaterial;
        private List<Hitbox> hitboxes = new List<Hitbox>();

        private void Awake()
        {
            // Setup hitboxes.
            Hitbox[] hb = GetComponentsInChildren<Hitbox>();
            foreach (Hitbox hitbox in hb)
                hitboxes.Add(hitbox);

            OnAwake();
        }

        public virtual void OnAwake() { }

        // Round entity's position to be by 1 decimal place.
        public void SnapPosition()
        {
            float x = Mathf.Round(transform.position.x * 10f) * 0.1f;
            float y = Mathf.Round(transform.position.y * 10f) * 0.1f;
            float z = Mathf.Round(transform.position.z * 10f) * 0.1f;
            transform.position = new Vector3(x, y, z);
        }

        public Vector3 Pos()
        {
            return transform.position;
        }

        public Hitbox FindHitbox(string hitboxName)
        {
            foreach(Hitbox hitbox in hitboxes)
            {
                if(hitbox.GetHitboxName() == hitboxName)
                    return hitbox;
            }

            print("Failed to find hitbox with name: " + hitboxName);
            return null;
        }

        public GameObject GetHitboxDisplayPrefab() { return hitboxDisplayPrefab; }
        public List<Hitbox> GetHitboxesList() {  return hitboxes; }

        public Material GetNormalMaterial() { return normalMaterial; }
        public Material GetAttackMaterial() { return attackMaterial; }
    }
}