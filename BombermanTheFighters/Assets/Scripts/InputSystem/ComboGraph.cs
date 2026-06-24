// LOVEEVIXEN
using System.Collections.Generic;
using UnityEngine;

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
        }

        public Branch[] branches;
    }
}