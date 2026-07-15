// LOVEEVIXEN
using EntitySystem;
using InputSystem;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager instance;

    private bool initiated = false;
    private List<Frame> frames = new List<Frame>();
    private int currentFrame = 0;
    private Player[] players = new Player[2];
    private Transform centerPos;
    [SerializeField] bool flipCamera;
    private SessionUI sessionUI;
    private bool offlineSession;

    [Header("Player Spawn Settings")]
    [SerializeField] string playerPrefabPath = "Player";
    [SerializeField][Range(1f, 10f)] float spawnGapBetweenOpponents = 9f;
    private int clientsLoaded;

    [Header("Player Positioning")]
    [SerializeField] float minPlayerDistance = 0.5f;
    [SerializeField] float maxPlayerDistance = 30f;
    private float startPlayerDistance;
    private bool allPlayersLoaded;

    // Match and round settings.
    private bool roundBegun = false;
    private bool endMatch = false;

    // Attack data.
    [System.Serializable]
    public class RegisteredHit
    {
        private EntityHitbox target;
        private AttackType attackType;
        private HitData hitData;

        public RegisteredHit(HitData hitData)
        {
            this.target = EntityHitbox.FromID(hitData.hitboxID);
            this.attackType = (AttackType)hitData.attackType;
            this.hitData = hitData;
        }

        public EntityHitbox GetTarget() { return target; }
        public AttackType GetAttackType() { return attackType; }

        public HitData GetHitData() { return hitData; }
    }
    private List<RegisteredHit> registeredHits = new List<RegisteredHit>();

    private void Awake()
    {
        instance = this;
        sessionUI = FindFirstObjectByType<SessionUI>();
        offlineSession = PhotonNetwork.OfflineMode;
    }

    void FixedUpdate()
    {
        if (initiated && allPlayersLoaded && !endMatch)
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

    // Session setup.
    #region
    void Initiate()
    {
        if (!initiated)
        {
            centerPos = GameObject.Find("PlayerCenterPosition").transform;

            // Spawn players.
            GameObject player1Obj = null;
            GameObject player2Obj = null;
            Vector3 player1Spawn = new Vector3((-spawnGapBetweenOpponents / 2f), 0f, 0f);
            Vector3 player2Spawn = new Vector3((spawnGapBetweenOpponents / 2f), 0f, 0f);
            startPlayerDistance = spawnGapBetweenOpponents;

            if (offlineSession)
            {
                // Instantiate players for offline play.
                player1Obj = PhotonNetwork.Instantiate(playerPrefabPath, player1Spawn, Quaternion.Euler(0f, 90f, 0f));
                player2Obj = PhotonNetwork.Instantiate(playerPrefabPath, player2Spawn, Quaternion.Euler(0f, 270f, 0f));
                
                // Setup local player numbers for each local player to control their own character.
                PlayerController player1Ctrl = player1Obj.GetComponent<PlayerController>();
                PlayerController player2Ctrl = player2Obj.GetComponent<PlayerController>();
                player1Ctrl.ReadFromInputData(InputReader.Player1());
                player2Ctrl.ReadFromInputData(InputReader.Player2());
            }
            else
            {
                // Instantiate local client's player for online play using player 1's controls.
                if (PhotonNetwork.IsMasterClient)
                {
                    player1Obj = PhotonNetwork.Instantiate(playerPrefabPath, player1Spawn, Quaternion.Euler(0f, 90f, 0f));
                    PlayerController playerCtrl = player1Obj.GetComponent<PlayerController>();
                    playerCtrl.ReadFromInputData(InputReader.Player1());
                }
                else
                {
                    player2Obj = PhotonNetwork.Instantiate(playerPrefabPath, player2Spawn, Quaternion.Euler(0f, 270f, 0f));
                    PlayerController playerCtrl = player2Obj.GetComponent<PlayerController>();
                    playerCtrl.ReadFromInputData(InputReader.Player1());
                }
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
        if (offlineSession)
        {
            for(int i = 0; i < players.Length; i++)
            {
                if (players[i] == null)
                {
                    players[i] = player;
                    if(i == 0)
                        player.LoadCharacter("Shirobon");
                    else if(i == 1)
                        player.LoadCharacter("Kurobon");

                    break;
                }
            }
        }
        else
        {
            if (player.photonView.Owner.IsMasterClient)
            {
                players[0] = player;
                player.LoadCharacter("Shirobon");
            }
            else
            {
                players[1] = player;
                player.LoadCharacter("Kurobon");
            }
        }

        // Check that all players have been loaded in.
        bool confirmedAllPlayersLoaded = true;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == null)
            {
                confirmedAllPlayersLoaded = false;
                break;
            }
        }

        if (confirmedAllPlayersLoaded)
            allPlayersLoaded = true;
    }
    #endregion

    public void EndMatch()
    {
        endMatch = true;
    }

    // See what hits registered in the last frame and make them take effect.
    public void ApplyRegisteredHits()
    {
        foreach (RegisteredHit registeredHit in registeredHits)
        {
            Entity hitEntity = registeredHit.GetTarget().GetEntity();
            if (hitEntity.photonView.IsMine)
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

        // Clear up previous frames if there becomes too many.
        int maxRecordedFrames = 1000;
        if(frames.Count > maxRecordedFrames)
        {
            int removeCount = frames.Count - maxRecordedFrames;
            frames.RemoveRange(0, removeCount);
        }

        return frame;
    }

    public void AddRegisteredHit(HitData hitData)
    {
        registeredHits.Add(new RegisteredHit(hitData));
    }

    public void AddLoadedClient()
    {
        clientsLoaded++;
        if (clientsLoaded >= PhotonNetwork.CurrentRoom.Players.Count)
            Initiate();
    }

    IEnumerator BeginRound()
    {
        // Set 'opponent' as the opposing player for each player.
        while (!allPlayersLoaded)
            yield return new WaitForSeconds(0.01f);

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
