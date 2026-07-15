// LOVEEVIXEN
using UnityEngine;
using UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("Nickname"))
            SetNickname(PlayerPrefs.GetString("Nickname"));
        else
            SetNickname(GenerateRandomNickname());
    }

    public void HostOfflineSession()
    {
        // Make sure client has a nickname before being present in a session.
        if (PhotonNetwork.NickName == "")
            SetNickname(GenerateRandomNickname());

        Menu[] menus = FindObjectsByType<Menu>(FindObjectsSortMode.None);
        foreach (Menu menu in menus)
            menu.Close();

        StartCoroutine(IHostOfflineSession());
    }

    IEnumerator IHostOfflineSession()
    {
        PhotonNetwork.OfflineMode = true;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        yield return new WaitForSeconds(0.01f);
        PhotonNetwork.CreateRoom(Application.productName, roomOptions);
        PhotonNetwork.LoadLevel("Game");
    }

    public void HostSession()
    {
        // Make sure client has a nickname before being present in a session.
        if (PhotonNetwork.NickName == "")
            SetNickname(GenerateRandomNickname());

        StartCoroutine(IHostSession());
    }

    IEnumerator IHostSession()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        yield return new WaitForSeconds(0.01f);
        PhotonNetwork.CreateRoom(PhotonNetwork.NickName, roomOptions);
    }

    public void JoinSession(string joinRoomName)
    {
        // Make sure client has a nickname before being present in a session.
        if (PhotonNetwork.NickName == "")
            SetNickname(GenerateRandomNickname());

        StartCoroutine(IJoinSession(joinRoomName));
    }

    IEnumerator IJoinSession(string joinRoomName)
    {
        yield return new WaitForSeconds(0.01f);

        if(joinRoomName != "")
            PhotonNetwork.JoinRoom(joinRoomName);
        else
        {
            NetworkFailurePrompt.instance.GetMenu().Open();
            NetworkFailurePrompt.instance.DisplayFailure("Failed to join session.", "Please enter a room name.");
        }
    }

    public void LeaveSession()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void SetNickname(string setNickname)
    {
        if (setNickname != "")
            PhotonNetwork.NickName = setNickname;
        else
            PhotonNetwork.NickName = GenerateRandomNickname();

        PlayerPrefs.SetString("Nickname", PhotonNetwork.NickName);
    }

    public string GenerateRandomNickname()
    {
        string setNickname = "Player " + Random.Range(0, 10000);
        return setNickname;
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        NetworkSessionLobby lobby = FindFirstObjectByType<NetworkSessionLobby>();
        if(lobby)
            lobby.GetMenu().Open();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        NetworkSessionLobby lobby = FindFirstObjectByType<NetworkSessionLobby>();
        if (lobby)
            lobby.GetMenu().Open();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        NetworkFailurePrompt.instance.GetMenu().Open();
        NetworkFailurePrompt.instance.DisplayFailure("Failed to create session.", message + ". Return code " + returnCode);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        NetworkFailurePrompt.instance.GetMenu().Open();
        NetworkFailurePrompt.instance.DisplayFailure("Failed to join session.", message + ". Return code " + returnCode);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        // End match if one was running.
        if (SessionManager.instance != null)
        {
            PhotonNetwork.LeaveRoom();
            SessionManager.instance.EndMatch();
            NetworkFailurePrompt.instance.GetMenu().Open();
            NetworkFailurePrompt.instance.DisplayFailure("Disconnected from session.", "Client has left.");
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        PhotonNetwork.LeaveRoom();
        NetworkFailurePrompt.instance.GetMenu().Open();
        NetworkFailurePrompt.instance.DisplayFailure("Disconnected from session.", "Host has left.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        if(cause != DisconnectCause.DisconnectByClientLogic)
        {
            // Close out of lobby if it is open.
            if (NetworkSessionLobby.instance != null)
                NetworkSessionLobby.instance.GetMenu().Close();

            // End match if one was running.
            if (SessionManager.instance != null)
                SessionManager.instance.EndMatch();

            // Show client reason for shutdown if it was an error.
            NetworkFailurePrompt.instance.GetMenu().Open();
            NetworkFailurePrompt.instance.DisplayFailure("Network error.", cause.ToString());
        }
    }
}
