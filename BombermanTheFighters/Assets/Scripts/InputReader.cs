// LOVEEVIXEN
using UnityEngine;

namespace InputSystem
{
    public class InputReader : MonoBehaviour
    {
        public const float joystickDead = 0.9f;
        public const float joystickDeadDiagonal = 0.4f;

        public class PlayerInputReader
        {
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
        }

        private static PlayerInputReader player1 = new PlayerInputReader();
        private static PlayerInputReader player2 = new PlayerInputReader();

        public static PlayerInputReader Player1() { return player1; }
        public static PlayerInputReader Player2() { return player2; }
    }
}