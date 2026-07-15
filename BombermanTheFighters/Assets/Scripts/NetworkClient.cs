// LOVEEVIXEN
using ChatSystem;
using Photon.Pun;
using UnityEngine;

public class NetworkClient : MonoBehaviourPunCallbacks
{
    [SerializeField] bool inGameScene = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (inGameScene)
            photonView.RPC("AddLoadedClient", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void AddLoadedClient()
    {
        SessionManager.instance.AddLoadedClient();
    }

    [PunRPC]
    void BeginMatch()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    [PunRPC]
    void RPC_SendMessage(string sender, string msg, float senderR, float senderG, float senderB, float msgR, float msgG, float msgB)
    {
        Color senderColor = new Color(senderR, senderG, senderB);
        Color msgColor = new Color(msgR, msgG, msgB);
        TextChat.messages.Add(new ChatMessage(sender, msg, senderColor, msgColor));

        // Clear up older messages if list gets too long.
        if (TextChat.messages.Count > TextChat.maxRecordedMessages)
        {
            int removeCount = TextChat.messages.Count - TextChat.maxRecordedMessages;
            TextChat.messages.RemoveRange(0, removeCount);
        }
    }
}
