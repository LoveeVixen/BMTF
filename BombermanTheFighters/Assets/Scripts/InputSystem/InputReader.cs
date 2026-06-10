// LOVEEVIXEN
using UnityEngine;

namespace InputSystem
{
    public class InputReader : MonoBehaviour
    {
        public const float joystickDead = 0.9f;
        public const float joystickDeadDiagonal = 0.4f;

        private static PlayerInputData player1 = new PlayerInputData();
        private static PlayerInputData player2 = new PlayerInputData();

        public static PlayerInputData Player1() { return player1; }
        public static PlayerInputData Player2() { return player2; }
    }
}