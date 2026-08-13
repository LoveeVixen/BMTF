// LOVEEVIXEN
using Audio;
using ChatSystem;
using Photon.Pun;
using UI;
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

        // Show newly received message in chat history UI.
        TextChatUI.instance.ResetChatHistoryFade();
        TextChatUI.instance.UpdateChatHistory();
    }

    [PunRPC]
    void RPC_NextRound()
    {
        SessionManager.instance.Continue_RPC_NextRound();
    }

    [PunRPC]
    void RPC_EndRound(int[] winnersIndex, int[] knockoutIndex, int announcementMsgIndex)
    {
        // Give win points to players that won.
        for (int i = 0; i < winnersIndex.Length; i++)
            SessionManager.instance.GetParticipate(winnersIndex[i]).Wins++;

        // Knockout players still standing that lost if they are not already knocked out.
        for (int i = 0; i < knockoutIndex.Length; i++)
        {
            if(SessionManager.instance.GetParticipate(knockoutIndex[i]).GetPlayer().photonView.IsMine)
                SessionManager.instance.GetParticipate(knockoutIndex[i]).GetPlayer().GetHealth().KnockOut();
        }

        if(announcementMsgIndex == 0)
        {
            SessionManager.instance.GetSessionUI().DisplayAnnouncement("K.O.");
            AudioManager.instance.PlayNonDiegeticSound("Announcer_KO");
        }
        else if(announcementMsgIndex == 1)
        {
            SessionManager.instance.GetSessionUI().DisplayAnnouncement("DOUBLE K.O.");
            AudioManager.instance.PlayNonDiegeticSound("Announcer_Double_KO");
        }
        else if (announcementMsgIndex == 2)
        {
            SessionManager.instance.GetSessionUI().DisplayAnnouncement("PERFECT");
            AudioManager.instance.PlayNonDiegeticSound("Announcer_Perfect");
        }
        else if (announcementMsgIndex == 3)
        {
            SessionManager.instance.GetSessionUI().DisplayAnnouncement("TIME UP");
            AudioManager.instance.PlayNonDiegeticSound("Announcer_Time_Up");
        }

        SessionManager.instance.GetSessionUI().DisplayCurrentWins();
        SessionManager.instance.PauseTimer = true;
    }

    [PunRPC]
    void RPC_ConcludeWinner(int winnerIndex)
    {
        SessionManager.instance.StartCoroutine(SessionManager.instance.IContinue_RPC_ConcludeWinner(winnerIndex));
    }

    [PunRPC]
    void RPC_ExitGameScene()
    {
        AudioManager.instance.StopMusic();

        if (SessionManager.instance.GetDoAfterMatch() == SessionManager.DoAfterMatch.index)
        {
            PhotonNetwork.Disconnect();
            PhotonNetwork.LoadLevel("Index");
        }
        else if (SessionManager.instance.GetDoAfterMatch() == SessionManager.DoAfterMatch.onlineLobby)
        {
            NetworkManager.instance.OpenLobbyOnLoadOnlineScene = true;
            PhotonNetwork.LoadLevel("Online");
        }
    }
}
