// LOVEEVIXEN
using UnityEngine;
using System.Collections.Generic;

namespace InputSystem
{
    public class InputReader : MonoBehaviour
    {
        public const float joystickDead = 0.49f;

        private static PlayerInputData player1 = new PlayerInputData();
        private static PlayerInputData player2 = new PlayerInputData();

        public static PlayerInputData Player1() { return player1; }
        public static PlayerInputData Player2() { return player2; }

        public static PlayerInputData[] AllPlayersInputData()
        {
            List<PlayerInputData> list = new List<PlayerInputData>();
            list.Add(Player1());
            list.Add(Player2());
            return list.ToArray();
        }

        // Player 1 keyboard input.
        public static KeyCode upP1 = KeyCode.W;
        public static KeyCode rightP1 = KeyCode.D;
        public static KeyCode downP1 = KeyCode.S;
        public static KeyCode leftP1 = KeyCode.A;
        public static KeyCode zeroP1 = KeyCode.J;
        public static KeyCode oneP1 = KeyCode.K;
        public static KeyCode twoP1 = KeyCode.U;
        public static KeyCode threeP1 = KeyCode.I;
        public static KeyCode startP1 = KeyCode.Return;
        public static KeyCode selectP1 = KeyCode.Space;

        // Player 2 keyboard input.
        public static KeyCode upP2 = KeyCode.UpArrow;
        public static KeyCode rightP2 = KeyCode.RightArrow;
        public static KeyCode downP2 = KeyCode.DownArrow;
        public static KeyCode leftP2 = KeyCode.LeftArrow;
        public static KeyCode zeroP2 = KeyCode.Keypad2;
        public static KeyCode oneP2 = KeyCode.Keypad3;
        public static KeyCode twoP2 = KeyCode.Keypad5;
        public static KeyCode threeP2 = KeyCode.Keypad6;
        public static KeyCode startP2 = KeyCode.KeypadEnter;
        public static KeyCode selectP2 = KeyCode.KeypadPlus;
    }
}