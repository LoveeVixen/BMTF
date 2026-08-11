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
        public enum RequiredState { none, idle, dashForward, dashBackward, dashLeft, dashRight, waveDash, weldBomb, notWeldBomb, lay, rollForward };
        public RequiredState requiredState;
        public bool avoidPlayerStateUpdate = false;
        public bool avoidApplyingAttackCooldown = false;

        // Play attack sound.
        public enum PlaySoundOnExecute { none, playCharacterAttack, playCharacterSpecial, playOverrideSound };
        public PlaySoundOnExecute playSoundOnExecute = PlaySoundOnExecute.playCharacterAttack;
        public string playOverrideSound;

        [Header("Attack stroll (Player movement while attacking)")]
        public bool enableAttackStroll = true;
        public float attackStrollSpeed = 1f;

        [Header("Opponent hit effect")]
        public float damage = 10f;
        public float stumbleSpeed = 1f;
        public float yVelocityLaunch = 0f;
        public float stumbleTime = 0.3f;
        public AttackType attackType;
        public bool canGuard = true;

        [Header("Apply effect status on hit")]
        public int applyEffectStatus = -1;
        public int effectStatusMultiply = 1;
        public float effectStatusLastTime = 1f;

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