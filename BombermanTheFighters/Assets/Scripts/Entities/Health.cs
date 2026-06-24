// LOVEEVIXEN
using UnityEngine;

namespace EntitySystem
{
    public class Health : MonoBehaviour
    {
        private float health = 100f;
        private float maxHealth = 100f;

        // Add or remove health from entity.
        public void AddHealth(float amount)
        {
            health += amount;

            if(amount > 0)
            {
                // Entity gained health.

                // Prevent health going over maximum.
                if(health > maxHealth)
                    health = maxHealth;
            }
            else
            {
                // Entity took damage.

                // Prevent health going below zero.
                if(health < 0)
                    health = 0;
            }
        }
    }
}