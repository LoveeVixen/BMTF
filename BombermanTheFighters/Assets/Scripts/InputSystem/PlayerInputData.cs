// LOVEEVIXEN
using UnityEngine;

namespace InputSystem
{
    [System.Serializable]
    public class PlayerInputData
    {
        // Holding input.
        public bool holdingUp;
        public bool holdingRight;
        public bool holdingDown;
        public bool holdingLeft;
        public bool holdingUpRight;
        public bool holdingDownRight;
        public bool holdingDownLeft;
        public bool holdingUpLeft;

        public bool holding0;
        public bool holding1;
        public bool holding2;
        public bool holding3;

        public bool holdingStart;
        public bool holdingSelect;

        // Press/tap input.
        private bool regPressingUp;
        public bool pressingUp;
        private bool regPressingRight;
        public bool pressingRight;
        private bool regPressingDown;
        public bool pressingDown;
        private bool regPressingLeft;
        public bool pressingLeft;
        private bool regPressingUpRight;
        public bool pressingUpRight;
        private bool regPressingDownRight;
        public bool pressingDownRight;
        private bool regPressingDownLeft;
        public bool pressingDownLeft;
        private bool regPressingUpLeft;
        public bool pressingUpLeft;

        public bool pressing0;
        public bool pressing1;
        public bool pressing2;
        public bool pressing3;

        public bool pressingStart;
        public bool pressingSelect;

        public static PlayerInputData CloneData(PlayerInputData original)
        {
            PlayerInputData clone = new PlayerInputData();

            clone.holdingUp = original.holdingUp;
            clone.pressingUp = original.pressingUp;
            clone.holdingRight = original.holdingRight;
            clone.pressingRight = original.pressingRight;
            clone.holdingDown = original.holdingDown;
            clone.pressingDown = original.pressingDown;
            clone.holdingLeft = original.holdingLeft;
            clone.pressingLeft = original.pressingLeft;
            clone.holdingUpRight = original.holdingUpRight;
            clone.pressingUpRight = original.pressingUpRight;
            clone.holdingDownRight = original.holdingDownRight;
            clone.pressingDownRight = original.pressingDownRight;
            clone.holdingDownLeft = original.holdingDownLeft;
            clone.pressingDownLeft = original.pressingDownLeft;
            clone.holdingUpLeft = original.holdingUpLeft;
            clone.pressingUpLeft = original.pressingUpLeft;
            clone.holding0 = original.holding0;
            clone.pressing0 = original.pressing0;
            clone.holding1 = original.holding1;
            clone.pressing1 = original.pressing1;
            clone.holding2 = original.holding2;
            clone.pressing2 = original.pressing2;
            clone.holding3 = original.holding3;
            clone.pressing3 = original.pressing3;
            clone.holdingStart = original.holdingStart;
            clone.pressingStart = original.pressingStart;
            clone.holdingSelect = original.holdingSelect;
            clone.pressingSelect = original.pressingSelect;

            return clone;
        }

        public int PressingInputCount()
        {
            int countedInputs = 0;
            if (pressingUp) countedInputs++;
            if (pressingRight) countedInputs++;
            if (pressingDown) countedInputs++;
            if (pressingLeft) countedInputs++;
            if (pressingUpRight) countedInputs++;
            if (pressingDownRight) countedInputs++;
            if (pressingDownLeft) countedInputs++;
            if (pressingUpLeft) countedInputs++;
            if (pressing0) countedInputs++;
            if (pressing1) countedInputs++;
            if (pressing2) countedInputs++;
            if (pressing3) countedInputs++;
            if (pressingStart) countedInputs++;
            if (pressingSelect) countedInputs++;

            return countedInputs;
        }

        public void PrintPressedInputs()
        {
            string printMessage = "";
            if (pressingUp) printMessage += "UP ";
            if (pressingRight) printMessage += "RIGHT ";
            if (pressingDown) printMessage += "DOWN ";
            if (pressingLeft) printMessage += "LEFT ";
            if (pressingUpRight) printMessage += "UP-RIGHT ";
            if (pressingDownRight) printMessage += "DOWN-RIGHT ";
            if (pressingDownLeft) printMessage += "DOWN-LEFT ";
            if (pressingUpLeft) printMessage += "UP-LEFT ";
            if (pressing0) printMessage += "0 ";
            if (pressing1) printMessage += "1 ";
            if (pressing2) printMessage += "2 ";
            if (pressing3) printMessage += "3 ";
            if (pressingStart) printMessage += "START ";
            if (pressingSelect) printMessage += "SELECT ";

            if(printMessage != "") Debug.Log(printMessage);
        }

        public bool RegPressingUp  { get { return regPressingUp; } set { regPressingUp = value; } }
        public bool RegPressingRight { get { return regPressingRight; } set { regPressingRight = value; } }
        public bool RegPressingDown { get { return regPressingDown; } set { regPressingDown = value; } }
        public bool RegPressingLeft { get { return regPressingLeft; } set { regPressingLeft = value; } }
        public bool RegPressingUpRight { get { return regPressingUpRight; } set { regPressingUpRight = value; } }
        public bool RegPressingDownRight { get { return regPressingDownRight; } set { regPressingDownRight = value; } }
        public bool RegPressingDownLeft { get { return regPressingDownLeft; } set { regPressingDownLeft = value; } }
        public bool RegPressingUpLeft { get { return regPressingUpLeft; } set { regPressingUpLeft = value; } }
    }
}