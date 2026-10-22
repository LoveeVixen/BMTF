// LOVEEVIXEN
using System.Collections.Generic;
using UnityEngine;
using static InputSystem.ComboInputData;

namespace InputSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ComboGraph", menuName = "Attack and Combo System/Character Move Set")]
    public class ComboGraph : ScriptableObject
    {
        [System.Serializable]
        public class Branch
        {
            public Attack attack;
            public Branch[] followUpCombos;
            public ComboInputData[] requiredInputs = new ComboInputData[1];
            public RequiredState requiredState;

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

            // Return the value of how many inputs are being used in this input data.
            public int InputAmount()
            {
                int amount = 0;
                for(int i = 0; i < requiredInputs.Length; i++)
                    amount += requiredInputs[i].InputAmount();

                if (requiredState != RequiredState.none) amount++;

                return amount;
            }
        }

        public Branch[] branches;
        public int[] ignoreFromUniversalMoveset; // Use index values to avoid loading combos from the universal movesets for specific characters.
    }
}