// LOVEEVIXEN
using EntitySystem;
using Fusion;
using Fusion.Sockets;
using InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class SessionManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static SessionManager instance;

    private bool initiated = false;
    private List<Frame> frames = new List<Frame>();
    private int currentFrame = 0;
    private Player[] players = new Player[2];
    private Transform centerPos;
    [SerializeField] bool flipCamera;
    private bool roundBegun = false;
    private SessionUI sessionUI;
    private NetworkRunner runner;
    private bool offlineSession;

    [Header("Player Spawn Settings")]
    [SerializeField] NetworkObject playerPrefab;
    [SerializeField][Range(1f, 10f)] float spawnGapBetweenOpponents = 9f;

    [Header("Player Positioning")]
    [SerializeField] float minPlayerDistance = 0.5f;
    [SerializeField] float maxPlayerDistance = 30f;
    private float startPlayerDistance;

    // Attack data.
    [System.Serializable]
    public class RegisteredHit
    {
        private EntityHitbox target;
        private AttackType attackType;
        private NetworkHitData hitData;

        public RegisteredHit(NetworkHitData hitData)
        {
            this.target = EntityHitbox.FromID(hitData.hitboxID);
            this.attackType = (AttackType)hitData.attackType;
            this.hitData = hitData;
        }

        public EntityHitbox GetTarget() { return target; }
        public AttackType GetAttackType() { return attackType; }

        public NetworkHitData GetHitData() { return hitData; }
    }
    private List<RegisteredHit> registeredHits = new List<RegisteredHit>();

    private void Awake()
    {
        instance = this;
        NetworkManager.instance.GetRunner().AddCallbacks(this);
        sessionUI = FindFirstObjectByType<SessionUI>();
        runner = NetworkManager.instance.GetRunner();
        offlineSession = runner.GameMode == GameMode.Single;
    }

    void FixedUpdate()
    {
        // Position player center reference, and rotate for camera reference.
        if (initiated)
        {
            if (roundBegun)
                ApplyRegisteredHits();
            
            // Calculate center position between both players.
            centerPos.transform.position = (players[0].Pos() + players[1].Pos()) / 2f;

            // Manipulate camera movement.
            if (!flipCamera)
                centerPos.LookAt(players[0].Pos());
            else
                centerPos.LookAt(players[1].Pos());

            centerPos.rotation = Quaternion.Euler(0f, centerPos.rotation.eulerAngles.y + 90f, 0f);
        }
    }

    void Update()
    {
        // Record tick for replay data.
        RecordFrame();
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public void OnInput(NetworkRunner runner, NetworkInput input) { }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Initiate();
    }

    public void OnSceneLoadStart(NetworkRunner runner) { }

    // Session setup.
    #region
    void Initiate()
    {
        if (!initiated)
        {
            centerPos = GameObject.Find("PlayerCenterPosition").transform;

            // Spawn players.
            NetworkObject player1Obj = null;
            NetworkObject player2Obj = null;
            PlayerRef hostPlayerRef = PlayerRef.FromIndex(1);
            PlayerRef clientPlayerRef = PlayerRef.None;
            Vector3 player1Spawn = new Vector3((-spawnGapBetweenOpponents / 2f), 0f, 0f);
            Vector3 player2Spawn = new Vector3((spawnGapBetweenOpponents / 2f), 0f, 0f);
            startPlayerDistance = spawnGapBetweenOpponents;

            if (offlineSession)
            {
                // Instantiate players for offline play.
                player1Obj = runner.Spawn(playerPrefab, player1Spawn, Quaternion.Euler(0f, 90f, 0f), hostPlayerRef);
                player2Obj = runner.Spawn(playerPrefab, player2Spawn, Quaternion.Euler(0f, 270f, 0f), hostPlayerRef);
                
                // Setup local player numbers for each local player to control their own character.
                PlayerController player1Ctrl = player1Obj.GetComponent<PlayerController>();
                PlayerController player2Ctrl = player2Obj.GetComponent<PlayerController>();
                player1Ctrl.LocalPlayerNumber = 1;
                player2Ctrl.LocalPlayerNumber = 2;
            }
            else if (NetworkManager.instance.GetRunner().IsServer)
            {
                // Find client player ref.
                foreach (PlayerRef playerRef in runner.ActivePlayers)
                {
                    if (playerRef != hostPlayerRef)
                    {
                        clientPlayerRef = playerRef;
                        break;
                    }
                }

                // Instantiate players for online play, either side is controlled with player 1's inputs.
                player1Obj = runner.Spawn(playerPrefab, player1Spawn, Quaternion.Euler(0f, 90f, 0f), hostPlayerRef);
                player2Obj = runner.Spawn(playerPrefab, player2Spawn, Quaternion.Euler(0f, 270f, 0f), clientPlayerRef);
            }

            // Load stage.
            Stage.LoadStageIntoScene(Stage.Find("Debug"));

            // Begin first round.
            StartCoroutine(BeginRound());

            // Finish initiating.
            initiated = true;
        }
    }

    public void ConfirmLoadedPlayer(Player player)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == null)
            {
                players[i] = player;

                if (player.HasInputAuthority)
                {
                    if (i == 0)
                        player.RPC_LoadCharacter("Shirobon");
                    else
                        player.RPC_LoadCharacter("Kurobon");
                }

                return;
            }
        }
    }
    #endregion

    // See what hits registered in the last frame and make them take effect.
    public void ApplyRegisteredHits()
    {
        foreach (RegisteredHit registeredHit in registeredHits)
        {
            Entity hitEntity = registeredHit.GetTarget().GetEntity();
            if (hitEntity.HasInputAuthority)
            {
                EntityHitbox hitbox = registeredHit.GetTarget();

                // Check that hit target is a player.
                Player player = hitEntity as Player;
                if (player != null)
                {
                    player.SetStumbleTimer(registeredHit.GetHitData().stumbleTime);
                    player.SetStumbleDirection(registeredHit.GetHitData().stumbleDirection);
                    player.SetStumbleSpeed(registeredHit.GetHitData().stumbleSpeed);
                    player.SetYVelocity(registeredHit.GetHitData().yVelocityLaunch);

                    if (registeredHit.GetAttackType() == AttackType.stumble)
                        player.HighHit();
                    else if (registeredHit.GetAttackType() == AttackType.launch)
                        player.LaunchHit();
                }
            }
        }

        registeredHits.RemoveRange(0, registeredHits.Count);
    }

    Frame RecordFrame()
    {
        Frame frame = new Frame();

        // Add frame to frames list.
        currentFrame++;
        frames.Add(frame);
        return frame;
    }

    public void AddRegisteredHit(NetworkHitData hitData)
    {
        registeredHits.Add(new RegisteredHit(hitData));
    }

    IEnumerator BeginRound()
    {
        yield return new WaitForSeconds(0.25f);

        // Set 'opponent' as the opposing player for each player.
        foreach (Player player in players)
            player.SetOpponent();

        sessionUI.DisplayAnnouncement("Round 1");

        yield return new WaitForSeconds(1f);

        sessionUI.DisplayAnnouncement("Round 1", "Ready?");

        yield return new WaitForSeconds(1f);

        sessionUI.DisplayAnnouncement("Fight!");

        yield return new WaitForSeconds(0.5f);

        sessionUI.ClearAnnouncement();
        roundBegun = true;
    }

    public bool HasInitiated() { return initiated; }
    public int GetCurrentFrame() {  return currentFrame; }
    public Player GetPlayer(int index) { return players[index]; }
    public Transform GetPlayerCenterPos() { return centerPos; }
    public bool GetFlipCamera() {  return flipCamera; }
    public float PlayerDistance() { return Vector3.Distance(players[0].Pos(), players[1].Pos()); }
    public float GetMinPlayerDistance() {  return minPlayerDistance; }
    public float GetMaxPlayerDistance() { return maxPlayerDistance; }
    public float GetStartPlayerDistance() { return startPlayerDistance; }
    public bool HasRoundBegun() {  return roundBegun; }
}
