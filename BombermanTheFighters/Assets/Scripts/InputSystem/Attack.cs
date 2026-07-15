// LOVEEVIXEN
using System.Collections.Generic;
using UnityEngine;

namespace InputSystem
{
    public enum AttackType { stumble, launch, stun };

    [CreateAssetMenu(fileName = "Attack", menuName = "Attack and Combo System/Attack", order = 1)]
    public class Attack : ScriptableObject
    {
        public string playAnimation = "";
        public ComboInputData[] requiredInputs = new ComboInputData[1];
        public bool avoidPlayerStateUpdate = false;
        public bool avoidApplyingAttackCooldown = false;

        [Header("Attack stroll (Player movement while attacking)")]
        public bool enableAttackStroll = true;

        [Header("Opponent hit effect")]
        public float stumbleSpeed = 1f;
        public float yVelocityLaunch = 0f;
        public float stumbleTime = 0.3f;
        public AttackType attackType;

        [Header("While facing left")]
        public string playLeftFacingAnimation = "";

        public bool MatchesRequiredInputs(List<ComboInputData> inputsList)
        {
            int matchingInputs = 0;
            ComboInputData[] inputs = inputsList.ToArray();
            int playerInputsIndex = inputs.Length;
            int requiredInputsIndex = requiredInputs.Length;

            // Early return if no inputs.
            if (inputs.Length == 0)
                return false;

            // Check that the player's most recent inputs match any of the combo requirements to execute.
            for (int i = requiredInputsIndex - 1; i >= 0; i--)
            {
                // Early return if there is not enough player inputs.
                if ((playerInputsIndex - 1) < 0)
                    return false;

                // Matches input in required inputs list.
                if (inputs[playerInputsIndex - 1].MatchesRequiredInput(requiredInputs[i]))
                {
                    matchingInputs++;
                    playerInputsIndex--;
                }
            }

            return matchingInputs == requiredInputs.Length;
        }
    }
}