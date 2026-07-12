// LOVEEVIXEN
using UnityEngine;
using Fusion;
using WebSocketSharp;
using Fusion.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkManager instance;
    
    [SerializeField] NetworkRunner runnerPrefab;
    private NetworkRunner runner;
    [SerializeField] NetworkObject networkDataSyncPrefab;
    private string nicknameOnJoin = "";

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

    void OnDestroy()
    {
        // Prevents memory leaks.
        if (runner != null)
            runner.RemoveCallbacks(this);
    }

    public async Task HostOfflineSession()
    {
        // Make sure client has a nickname before being present in a session.
        if (nicknameOnJoin.IsNullOrEmpty())
            nicknameOnJoin = SetRandomNickname();

        // Setup new network runner.
        if (runner == null)
            InstantiateNewNetworkRunner();

        // Attempt to begin an offline session.
        StartGameArgs args = new StartGameArgs();
        args.GameMode = GameMode.Single;
        args.SessionName = Application.productName;
        args.PlayerCount = 2;
        args.Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        args.SceneManager = runner.GetComponent<NetworkSceneManagerDefault>();

        // Start offline session.
        var result = await runner.StartGame(args);

        if (result.Ok)
        {
            // Session creation was a success.
            await runner.LoadScene("Game");
        }
        else
        {
            // Session creation failed.
        }
    }

    public async Task HostSession()
    {
        // Make sure client has a nickname before being present in a session.
        if (nicknameOnJoin.IsNullOrEmpty())
            nicknameOnJoin = SetRandomNickname();

        // Setup new network runner.
        if(runner == null)
            InstantiateNewNetworkRunner();

        // Attempt to host session.
        StartGameArgs args = new StartGameArgs();
        args.GameMode = GameMode.Host;
        args.SessionName = nicknameOnJoin;
        args.PlayerCount = 2;
        args.Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        args.SceneManager = runner.GetComponent<NetworkSceneManagerDefault>();

        // Host session.
        var result = await runner.StartGame(args);

        if(result.Ok)
        {
            // Session creation was a success.
            NetworkSessionLobby.instance.GetMenu().Open();
            NetworkSessionLobby.instance.UpdateSessionNameText();
        }
        else
        {
            // Session creation failed.
            NetworkFailurePrompt.instance.GetMenu().Open();
            NetworkFailurePrompt.instance.DisplayFailure("Failed to create session.", result.ShutdownReason.ToString());

            if (runner.IsRunning)
            {
                await runner.Shutdown();
                Destroy(runner.gameObject);
            }
        }
    }

    public async Task JoinSession(string joinRoomName)
    {
        // Make sure client has a nickname before being present in a session.
        if (nicknameOnJoin.IsNullOrEmpty())
            nicknameOnJoin = SetRandomNickname();

        // Setup new network runner.
        if (runner == null)
            InstantiateNewNetworkRunner();

        // Attempt to join session.
        StartGameArgs args = new StartGameArgs();
        args.GameMode = GameMode.Client;
        args.SessionName = joinRoomName;
        args.PlayerCount = 2;
        args.Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        args.SceneManager = runner.GetComponent<NetworkSceneManagerDefault>();

        // Join session.
        var result = await runner.StartGame(args);

        if (result.Ok)
        {
            // Session join was a success.
            NetworkSessionLobby.instance.GetMenu().Open();
            NetworkSessionLobby.instance.UpdateSessionNameText();
        }
        else
        {
            // Session join failed.
            NetworkFailurePrompt.instance.GetMenu().Open();
            NetworkFailurePrompt.instance.DisplayFailure("Failed to join session.", result.ShutdownReason.ToString());

            if (runner.IsRunning)
            {
                await runner.Shutdown();
                Destroy(runner.gameObject);
            }
        }
    }

    public async Task LeaveSession()
    {
        if(runner != null && runner.IsRunning)
        {
            await runner.Shutdown();
            runner.RemoveCallbacks(this);
            Destroy(runner.gameObject);
        }
    }

    NetworkRunner InstantiateNewNetworkRunner()
    {
        if (runner == null)
        {
            NetworkRunner nr = Instantiate(runnerPrefab);
            runner = nr;
            DontDestroyOnLoad(nr.gameObject);
            runner.AddCallbacks(this);
            return nr;
        }
        else
        {
            Debug.Log("Network runner already exists.");
            return null;
        }
    }

    NetworkDataSync InstantiateNewNetworkDataSync(PlayerRef player)
    {
        NetworkObject networkDataSyncObj = runner.Spawn(networkDataSyncPrefab, Vector3.zero, Quaternion.identity, player);
        NetworkDataSync dataSync = networkDataSyncObj.GetComponent<NetworkDataSync>();

        if(player == PlayerRef.FromIndex(1))
            dataSync.RPC_SetStartFacingRight(true);

        return dataSync;
    }

    string SetRandomNickname()
    {
        string setNickname = "Player " + Random.Range(0, 10000);
        return setNickname;
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if(runner.IsServer)
            InstantiateNewNetworkDataSync(player);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // Delete the NetworkDataSync object belonging to the client that just left.
        NetworkDataSync[] dataSyncs = FindObjectsByType<NetworkDataSync>(FindObjectsSortMode.None);
        foreach(NetworkDataSync dataSync in dataSyncs)
        {
            NetworkObject netObj = dataSync.GetComponent<NetworkObject>();
            if (netObj.InputAuthority == player)
            {
                runner.Despawn(netObj);
                return;
            }
        }
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        // Check that the shutdown reason was by error.
        if(shutdownReason != ShutdownReason.Ok)
        {
            // Close out of lobby if it is open.
            if(NetworkSessionLobby.instance != null)
                NetworkSessionLobby.instance.GetMenu().Close();

            // Show client reason for shutdown if it was an error.
            NetworkFailurePrompt.instance.GetMenu().Open();
            NetworkFailurePrompt.instance.DisplayFailure("Disconnected from session", shutdownReason.ToString());
        }
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public void OnInput(NetworkRunner runner, NetworkInput input) { }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnSceneLoadDone(NetworkRunner runner) { }

    public void OnSceneLoadStart(NetworkRunner runner) { }

    public string NicknameOnJoin
    {
        get { return nicknameOnJoin; }
        set
        { 
            nicknameOnJoin = value;
            if(nicknameOnJoin.IsNullOrEmpty()) nicknameOnJoin = SetRandomNickname();
        }
    }

    public NetworkRunner GetRunner() { return runner; }
}
