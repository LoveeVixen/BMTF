// LOVEEVIXEN
using UnityEngine;
using System.Collections.Generic;

namespace InputSystem
{
    [System.Serializable]
    public class ComboReader
    {
        public List<ComboInputData> inputs = new List<ComboInputData>();

        public int RecentIndex()
        {
            return inputs.Count - 1;
        }

        public void Reset()
        {
            inputs.RemoveRange(0, inputs.Count);
        }
    }
}