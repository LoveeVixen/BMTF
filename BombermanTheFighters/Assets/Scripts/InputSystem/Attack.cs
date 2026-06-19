// LOVEEVIXEN
using System.Collections.Generic;

namespace InputSystem
{
    [System.Serializable]
    public class Attack
    {
        public string playAnimation = "NOCLIP";
        public bool isGapInput = false;
        public ComboInputData requiredInput = new ComboInputData();
        public List<Attack> nextCombos = new List<Attack>();
    }
}