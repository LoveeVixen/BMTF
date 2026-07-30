// LOVEEVIXEN
using Audio;
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
    private Transform centerPos;
    [SerializeField] bool flipCamera;
    private Cam cam;
    private SessionUI sessionUI;
    private bool offlineSession;

    [SerializeField] float timer = 60f;
    private bool pauseTimer = false;

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
    private int currentRound = 1;
    private bool roundBegun = false;
    private bool roundEnding = false;
    private bool roundEnded = false;
    private bool endSession = false;
    public enum DoAfterMatch { index, onlineLobby, gameOverMenu };
    private DoAfterMatch doAfterMatch = DoAfterMatch.index;

    public class Participate
    {
        private Player player;
        private int wins = 0;

        public Participate(Player player)
        {
            this.player = player;
        }

        public Player GetPlayer() { return player; }
        public int Wins {  get { return wins; } set { wins = value; } }
    }
    private Participate[] players = new Participate[2];
    private Vector3[] playerSpawns = new Vector3[2];
    private Quaternion[] playerStartEulerAngles = new Quaternion[2];

    // Attack data.
    [System.Serializable]
    public class RegisteredHit
    {
        private EntityHitbox target;
        private HitData hitData;

        public RegisteredHit(HitData hitData)
        {
            this.target = EntityHitbox.FromID(hitData.hitboxID);
            this.hitData = hitData;
        }

        public EntityHitbox GetTarget() { return target; }
        public HitData GetHitData() { return hitData; }
    }
    private List<RegisteredHit> registeredHits = new List<RegisteredHit>();

    private void Awake()
    {
        instance = this;
        cam = FindFirstObjectByType<Cam>();
        sessionUI = FindFirstObjectByType<SessionUI>();
        offlineSession = PhotonNetwork.OfflineMode;
        if (!offlineSession) doAfterMatch = DoAfterMatch.onlineLobby;

        playerSpawns[0] = new Vector3((-spawnGapBetweenOpponents / 2f), 0f, 0f);
        playerStartEulerAngles[0] = Quaternion.Euler(0f, 90f, 0f);
        playerSpawns[1] = new Vector3((spawnGapBetweenOpponents / 2f), 0f, 0f);
        playerStartEulerAngles[1] = Quaternion.Euler(0f, 270f, 0f);
    }

    void FixedUpdate()
    {
        if (roundBegun)
        {
            ApplyRegisteredHits();

            // Tick down timer until round is over.
            if (timer > 0f && !pauseTimer)
            {
                timer -= Time.deltaTime;
                if (timer < 0f)
                    timer = 0f;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                // Check to see if one of the players has yet been knocked out.
                foreach (Participate player in players)
                {
                    if (player.GetPlayer().GetHealth().IsKnockedOut() && !roundEnding)
                        EndRound(0.1f);
                }

                // Check that round timer has come to an end.
                if (timer == 0f && !roundEnding)
                    EndRound(0f);
            }
        }

        if (initiated && allPlayersLoaded && !endSession)
        {
            // Calculate center position between both players.
            centerPos.transform.position = (players[0].GetPlayer().Pos() + players[1].GetPlayer().Pos()) / 2f;

            // Manipulate camera movement.
            if (!flipCamera)
                centerPos.LookAt(players[0].GetPlayer().Pos());
            else
                centerPos.LookAt(players[1].GetPlayer().Pos());

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
            startPlayerDistance = spawnGapBetweenOpponents;

            if (offlineSession)
            {
                // Instantiate players for offline play.
                player1Obj = PhotonNetwork.Instantiate(playerPrefabPath, playerSpawns[0], playerStartEulerAngles[0]);
                player2Obj = PhotonNetwork.Instantiate(playerPrefabPath, playerSpawns[1], playerStartEulerAngles[1]);
                
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
                    player1Obj = PhotonNetwork.Instantiate(playerPrefabPath, playerSpawns[0], playerStartEulerAngles[0]);
                    PlayerController playerCtrl = player1Obj.GetComponent<PlayerController>();
                    playerCtrl.ReadFromInputData(InputReader.Player1());
                }
                else
                {
                    player2Obj = PhotonNetwork.Instantiate(playerPrefabPath, playerSpawns[1], playerStartEulerAngles[1]);
                    PlayerController playerCtrl = player2Obj.GetComponent<PlayerController>();
                    playerCtrl.ReadFromInputData(InputReader.Player1());
                }
            }

            // Load stage.
            Stage stage = Stage.Find("Snowy");
            Stage.LoadStageIntoScene(stage);
            AudioManager.instance.PlayMusic(stage.music);

            // Begin first round.
            BeginRound();

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
                    players[i] = new Participate(player);
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
                players[0] = new Participate(player);
                player.LoadCharacter("Shirobon");
            }
            else
            {
                players[1] = new Participate(player);
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

    // Round/session.
    #region

    void BeginRound()
    {
        StartCoroutine(IBeginRound());
    }

    IEnumerator IBeginRound()
    {
        string roundNumber = "ROUND " + currentRound;

        // Set 'opponent' as the opposing player for each player.
        while (!allPlayersLoaded)
            yield return new WaitForSeconds(0.01f);

        sessionUI.Initiate();
        foreach (Participate player in players)
            player.GetPlayer().SetOpponent();

        // Figure out if this is the final round or not. This adjusts the announcement message.
        bool isFinalRound = true;
        foreach (Participate player in players)
        {
            if (player.Wins != Gamerules.usingGamerules.rounds - 1)
                isFinalRound = false;
        }

        if (isFinalRound)
        {
            roundNumber = "FINAL ROUND";
            AudioManager.instance.PlayNonDiegeticSound("Announcer_Final_Round");
        }
        else
            AudioManager.instance.PlayNonDiegeticSound("Announcer_Round");

        sessionUI.DisplayAnnouncement(roundNumber);

        yield return new WaitForSeconds(0.5f);

        if(!isFinalRound)
        {
            if(currentRound == 1)
                AudioManager.instance.PlayNonDiegeticSound("Announcer_One");
            else if(currentRound == 2)
                AudioManager.instance.PlayNonDiegeticSound("Announcer_Two");
            else if (currentRound == 3)
                AudioManager.instance.PlayNonDiegeticSound("Announcer_Three");
            else if (currentRound == 4)
                AudioManager.instance.PlayNonDiegeticSound("Announcer_Four");
            else if (currentRound == 5)
                AudioManager.instance.PlayNonDiegeticSound("Announcer_Five");
            else if (currentRound == 6)
                AudioManager.instance.PlayNonDiegeticSound("Announcer_Six");
            else if (currentRound == 7)
                AudioManager.instance.PlayNonDiegeticSound("Announcer_Seven");
            else if (currentRound == 8)
                AudioManager.instance.PlayNonDiegeticSound("Announcer_Eight");
            else if (currentRound == 9)
                AudioManager.instance.PlayNonDiegeticSound("Announcer_Nine");
        }

        yield return new WaitForSeconds(1f);

        sessionUI.DisplayAnnouncement(roundNumber, "READY?");

        yield return new WaitForSeconds(1f);

        sessionUI.DisplayAnnouncement("FIGHT!");
        AudioManager.instance.PlayNonDiegeticSound("Announcer_Fight");

        yield return new WaitForSeconds(0.5f);

        sessionUI.ClearAnnouncement();
        roundBegun = true;
    }

    // Begin next round.
    public void NextRound()
    {
        NetworkClient networkClient = FindFirstObjectByType<NetworkClient>();
        networkClient.photonView.RPC("RPC_NextRound", RpcTarget.All);
    }

    // Continuation from 'RPC_NextRound()' in the NetworkClient class.
    public void Continue_RPC_NextRound()
    {
        currentRound++;

        // Reset variables in preparation for next round.
        timer = Gamerules.usingGamerules.roundTime;
        roundBegun = false;
        roundEnding = false;
        roundEnded = false;
        pauseTimer = false;
        cam.transform.position = cam.GetAwakePos();

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetPlayer().photonView.IsMine)
            {
                players[i].GetPlayer().GetHealth().ResetHealth();
                players[i].GetPlayer().Idle();
                players[i].GetPlayer().transform.position = playerSpawns[i];
                players[i].GetPlayer().transform.rotation = playerStartEulerAngles[i];
            }
        }

        // Clear up remaining projectiles.
        Bomb[] bombs = FindObjectsByType<Bomb>(FindObjectsSortMode.None);
        foreach (Bomb bomb in bombs)
        {
            bomb.Defuse();
            bomb.Destroy();
        }

        // Begin next round.
        BeginRound();
    }

    // Continuation from 'RPC_ConcludeWinner()' in the NetworkClient class.
    public void ConcludeWinner(int playerIndex)
    {
        NetworkClient networkClient = FindFirstObjectByType<NetworkClient>();
        networkClient.photonView.RPC("RPC_ConcludeWinner", RpcTarget.All, playerIndex);
    }

    public IEnumerator IContinue_RPC_ConcludeWinner(int playerIndex)
    {
        EndSession();
        yield return new WaitForSeconds(1f);

        if (playerIndex != -1)
        {
            if (offlineSession)
            {
                sessionUI.DisplayAnnouncement(players[playerIndex].GetPlayer().GetLoadedCharacter().name.ToUpper() + " WINS");

                // Play the sound for the winning player.
                AudioManager.Sound announceNameSound = AudioManager.instance.GetSound(players[playerIndex].GetPlayer().GetLoadedCharacter().announceNameSound);
                if (announceNameSound != null)
                    AudioManager.instance.PlayNonDiegeticSound(players[playerIndex].GetPlayer().GetLoadedCharacter().announceNameSound);

                yield return new WaitForSeconds(1f);

                AudioManager.instance.PlayNonDiegeticSound("Announcer_Win");
            }
            else
            {
                bool localClientWon = players[playerIndex].GetPlayer().photonView.IsMine;
                if(localClientWon)
                {
                    sessionUI.DisplayAnnouncement("YOU WIN");
                    AudioManager.instance.PlayNonDiegeticSound("Announcer_You_Win");
                }
                else
                {
                    sessionUI.DisplayAnnouncement("YOU LOSE");
                    AudioManager.instance.PlayNonDiegeticSound("Announcer_You_Lose");
                }
            }
        }
        else
        {
            sessionUI.DisplayAnnouncement("DRAW");
            AudioManager.instance.PlayNonDiegeticSound("Announcer_Draw");
        }

        yield return new WaitForSeconds(2f);

        ExitGameScene();
    }

    public void EndRound(float delayTime)
    {
        if (!roundEnding)
            StartCoroutine(IEndRound(delayTime));
        else
            Debug.Log("Round is already ending!");
    }

    IEnumerator IEndRound(float delayTime)
    {
        List<int> winningPlayerIndex = new List<int>();
        List<int> knockoutPlayerIndex = new List<int>();
        int announcementMsgIndex = -1; // 0 = K.O., 1 = Double K.O., 2 = Perfect, 3 = Time up.

        if (PhotonNetwork.IsMasterClient)
        {
            roundEnding = true;

            yield return new WaitForSeconds(delayTime);

            roundEnded = true;

            if (timer > 0f)
            {
                // Round end due to player knockout.

                // Check which players have been knocked out.
                int playersKnockedOut = 0;
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].GetPlayer().GetHealth().IsKnockedOut())
                        playersKnockedOut++;
                }

                if (playersKnockedOut >= players.Length)
                {
                    // All players knocked out. Each player gets a win point.
                    for (int i = 0; i < players.Length; i++)
                        players[i].Wins++;

                    // Display round outcome on UI.
                    announcementMsgIndex = 1;
                }
                else
                {
                    bool isPerfect = true;
                    for (int i = 0; i < players.Length; i++)
                    {
                        // Give a win point to the player not knocked out.
                        if (!players[i].GetPlayer().GetHealth().IsKnockedOut())
                        {
                            winningPlayerIndex.Add(i);

                            // Check that winning player had full health or not, making it a perfect.
                            if (players[i].GetPlayer().GetHealth().CurrentHealth < players[i].GetPlayer().GetHealth().GetMaxHealth())
                                isPerfect = false;
                        }
                    }

                    // Display round outcome on UI.
                    if(isPerfect)
                        announcementMsgIndex = 2;
                    else
                        announcementMsgIndex = 0;
                }
            }
            else
            {
                // Round end due to time running out.

                // Find which player has the most health.
                List<float> finalHealth = new List<float>();
                int highestFinalHealthIndex = -1;
                for (int i = 0; i < players.Length; i++)
                    finalHealth.Add(players[i].GetPlayer().GetHealth().CurrentHealth);

                for (int i = 0; i < finalHealth.Count; i++)
                {
                    bool notHighest = false;
                    for (int e = 0; e < finalHealth.Count; e++)
                    {
                        if (finalHealth[i] <= finalHealth[e] && e != i)
                            notHighest = true;
                    }

                    if (!notHighest)
                        highestFinalHealthIndex = i;
                }

                // Give a win point to the player with most health.
                if (highestFinalHealthIndex != -1)
                    winningPlayerIndex.Add(highestFinalHealthIndex);
                else
                {
                    // All players had equal health, and get a win point.
                    for (int i = 0; i < players.Length; i++)
                        winningPlayerIndex.Add(i);
                }

                // Knockout player that lost.
                for (int i = 0; i < players.Length; i++)
                {
                    if (i != highestFinalHealthIndex)
                        knockoutPlayerIndex.Add(i);
                }

                // Display round outcome on UI.
                announcementMsgIndex = 3;
            }

            NetworkClient networkClient = FindFirstObjectByType<NetworkClient>();
            networkClient.photonView.RPC("RPC_EndRound", RpcTarget.All, winningPlayerIndex.ToArray(), knockoutPlayerIndex.ToArray(), announcementMsgIndex);

            yield return new WaitForSeconds(3f);

            // Figure out which player is determined the winner of the overall game, or move onto next round.
            int finalWinners = 0; // 0 = No final winner, move onto next round. 1 = Final determined winner. 2+ = Game tie.
            int finalWinnerIndex = -1; // If 'finalWinners' is equal to 1, this determines which player won.
            int foundFinalWinnerIndex = -1;
            for (int i = 0; i < players.Length; i++)
            {
                // Check which players had won enough rounds to be declared winner. If there is more than one then game is declared a draw.
                if (players[i].Wins >= Gamerules.usingGamerules.rounds)
                {
                    finalWinners++;
                    foundFinalWinnerIndex = i;
                }
            }

            if (finalWinners == 1)
                finalWinnerIndex = foundFinalWinnerIndex;

            // Move onto next round, or conclude final winner.
            if (finalWinners == 0)
                NextRound();
            else
                ConcludeWinner(finalWinnerIndex);
        }
        else
            Debug.Log("Only host can end round!");
    }

    public void EndSession()
    {
        endSession = true;
    }

    public void ExitGameScene()
    {
        NetworkClient networkClient = FindFirstObjectByType<NetworkClient>();
        networkClient.photonView.RPC("RPC_ExitGameScene", RpcTarget.All);
    }
    #endregion

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
                    player.GetHealth().RemoveHealth(registeredHit.GetHitData().damage);
                    player.SetStumbleTimer(registeredHit.GetHitData().stumbleTime);
                    player.SetStumbleDirection(registeredHit.GetHitData().stumbleDirection);
                    player.SetStumbleSpeed(registeredHit.GetHitData().stumbleSpeed);
                    player.SetYVelocity(registeredHit.GetHitData().yVelocityLaunch);

                    if (registeredHit.GetHitData().attackType == (int)AttackType.stumble)
                        player.HighHit();
                    else if (registeredHit.GetHitData().attackType == (int)AttackType.launch)
                        player.LaunchHit();
                }
            }

            // Play hit sound.
            if(registeredHit.GetHitData().damage <= 10f && !hitEntity.GetHealth().IsKnockedOut())
                AudioManager.instance.PlaySound("Hit", hitEntity.Pos());
            else
                AudioManager.instance.PlaySound("Critical_Hit", hitEntity.Pos());
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

    public bool HasInitiated() { return initiated; }
    public bool PauseTimer { get { return pauseTimer; } set { pauseTimer = value; } }
    public int GetCurrentFrame() {  return currentFrame; }
    public Participate GetParticipate(int index) { return players[index]; }
    public Transform GetPlayerCenterPos() { return centerPos; }
    public bool GetFlipCamera() {  return flipCamera; }
    public float PlayerDistance() { return Vector3.Distance(players[0].GetPlayer().Pos(), players[1].GetPlayer().Pos()); }
    public float GetMinPlayerDistance() {  return minPlayerDistance; }
    public float GetMaxPlayerDistance() { return maxPlayerDistance; }
    public float GetStartPlayerDistance() { return startPlayerDistance; }
    public SessionUI GetSessionUI() { return sessionUI; }
    public bool HasRoundBegun() {  return roundBegun; }
    public float GetTimer() {  return timer; }
    public DoAfterMatch GetDoAfterMatch() { return doAfterMatch; }
}
