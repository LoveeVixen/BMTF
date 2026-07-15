// LOVEEVIXEN
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

namespace ChatSystem
{
    public class TextChat : MonoBehaviour
    {
        private static bool isOpen = false;

        // Input.
        public static KeyCode chatInput = KeyCode.T;
        public static KeyCode sendInput = KeyCode.Return;
        public static KeyCode closeInput = KeyCode.Escape;

        // Chat history.
        public static List<ChatMessage> messages = new List<ChatMessage>();
        public static int maxRecordedMessages = 1000;

        public static void SendChatMessage(string msg)
        {
            Color c = Color.white;
            NetworkClient networkClient = FindFirstObjectByType<NetworkClient>();
            networkClient.photonView.RPC("RPC_SendMessage", RpcTarget.All, PhotonNetwork.NickName, msg, c.r, c.g, c.b, c.r, c.g, c.b);
        }

        public static void SendChatMessage(string sender, string msg)
        {
            Color c = Color.white;
            NetworkClient networkClient = FindFirstObjectByType<NetworkClient>();
            networkClient.photonView.RPC("RPC_SendMessage", RpcTarget.All, sender, msg, c.r, c.g, c.b, c.r, c.g, c.b);
        }

        public static void SendChatMessage(string sender, string msg, Color senderColor, Color msgColor)
        {
            Color sc = senderColor;
            Color mc = msgColor;
            NetworkClient networkClient = FindFirstObjectByType<NetworkClient>();
            networkClient.photonView.RPC("RPC_SendMessage", RpcTarget.All, sender, msg, sc.r, sc.g, sc.b, mc.r, mc.g, mc.b);
        }

        public static void Open() { isOpen = true; }
        public static void Close() { isOpen = false; }
        public static bool IsOpen() {  return isOpen; }
    }
}