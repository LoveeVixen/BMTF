// LOVEEVIXEN
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

namespace ChatSystem
{
    public class TextChat
    {
        private static string savedMessage = "";

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
            NetworkClient networkClient = MonoBehaviour.FindFirstObjectByType<NetworkClient>();
            networkClient.photonView.RPC("RPC_SendMessage", RpcTarget.All, LocalNickname(), msg, c.r, c.g, c.b, c.r, c.g, c.b);
            savedMessage = "";
        }

        public static void SendChatMessage(string sender, string msg)
        {
            Color c = Color.white;
            NetworkClient networkClient = MonoBehaviour.FindFirstObjectByType<NetworkClient>();
            networkClient.photonView.RPC("RPC_SendMessage", RpcTarget.All, sender, msg, c.r, c.g, c.b, c.r, c.g, c.b);
            savedMessage = "";
        }

        public static void SendChatMessage(string sender, string msg, Color senderColor, Color msgColor)
        {
            Color sc = senderColor;
            Color mc = msgColor;
            NetworkClient networkClient = MonoBehaviour.FindFirstObjectByType<NetworkClient>();
            networkClient.photonView.RPC("RPC_SendMessage", RpcTarget.All, sender, msg, sc.r, sc.g, sc.b, mc.r, mc.g, mc.b);
            savedMessage = "";
        }

        // Basic return methods. Edit these to be based on that of which online-multiplayer service you're using.
        #region
        public static string LocalNickname() { return PhotonNetwork.NickName; }
        public static bool IsConnectedToSession() { return PhotonNetwork.InRoom && !PhotonNetwork.OfflineMode; }
        #endregion

        public static string SavedMessage { get { return savedMessage; } set { savedMessage = value; } }
    }
}