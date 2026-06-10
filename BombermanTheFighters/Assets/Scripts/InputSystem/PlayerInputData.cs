// LOVEEVIXEN

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
            clone.holdingRight = original.holdingRight;
            clone.holdingDown = original.holdingDown;
            clone.holdingLeft = original.holdingLeft;
            clone.holdingUpRight = original.holdingUpRight;
            clone.holdingDownRight = original.holdingDownRight;
            clone.holdingDownLeft = original.holdingDownLeft;
            clone.holdingUpLeft = original.holdingUpLeft;
            clone.holding0 = original.holding0;
            clone.holding1 = original.holding1;
            clone.holding2 = original.holding2;
            clone.holding3 = original.holding3;
            clone.holdingStart = original.holdingStart;
            clone.holdingSelect = original.holdingSelect;

            return clone;
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