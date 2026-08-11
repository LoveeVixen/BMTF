// LOVEEVIXEN
using UnityEngine;
using Photon.Pun;

namespace EntitySystem
{
    public class Health : MonoBehaviourPunCallbacks, IPunObservable
    {
        private float health = 256f;
        private float defaultHealth;
        private float maxHealth = 256f;
        private Entity entity;

        void Awake()
        {
            entity = GetComponent<Entity>();
            if(health > maxHealth) health = maxHealth;
            defaultHealth = health;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }

        // Add health to entity.
        public void AddHealth(float heal)
        {
            float amount = heal;

            // If the heal amount actually reduces health, then call the opposite function instead.
            if(amount < 0f)
            {
                RemoveHealth(Mathf.Abs(amount));
                return;
            }
            else
            {
                health += amount;

                // Prevent health going over maximum.
                if (health > maxHealth)
                    health = maxHealth;
            }
        }

        // Remove health from entity.
        public void RemoveHealth(float damage)
        {
            float amount = damage;

            // If the damage amount actually adds health, then call the opposite function instead.
            if (amount < 0f)
            {
                AddHealth(Mathf.Abs(amount));
                return;
            }
            else
            {
                if (!IsKnockedOut())
                {
                    health -= amount;

                    Player player = entity as Player;
                    if (health < 0)
                    {
                        // Entity has been knocked out.
                        health = 0;

                        // Play knockout sound.
                        if (player != null)
                            player.PlayVoice(player.GetLoadedCharacter().knockoutSound);
                    }
                    else
                    {
                        // Entity is not yet knocked out.
                    }
                }
            }
        }

        // Reset entity's health.
        public void ResetHealth()
        {
            health = defaultHealth;
        }

        // Set entity health to zero.
        public void KnockOut()
        {
            RemoveHealth(maxHealth);
        }

        public float CurrentHealth { get { return health; } set { health = value; } } // Only use this for getting, or when syncing client entity health values.
        public float GetMaxHealth() { return maxHealth; }
        public bool IsKnockedOut() {  return health <= 0f; }
    }
}