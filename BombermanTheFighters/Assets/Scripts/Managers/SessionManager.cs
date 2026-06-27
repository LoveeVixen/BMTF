// LOVEEVIXEN
using EntitySystem;
using InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SessionManager : MonoBehaviour
{
    public static SessionManager instance;

    private List<Frame> frames = new List<Frame>();
    private int currentFrame = 0;

    private Player player1;
    private Player player2;
    private Transform centerPos;
    [SerializeField] bool flipCamera;

    [Header("Player Positioning")]
    [SerializeField] float minPlayerDistance = 3f;
    [SerializeField] float maxPlayerDistance = 30f;
    private float startPlayerDistance;

    // Attack data.
    [System.Serializable]
    public class RegisteredHit
    {
        private Hitbox target;
        private Attack attack;
        private Vector3 stumbleDirection;
        private float yVelocityLaunch;

        public RegisteredHit(Hitbox target, Attack attack, Vector3 stumbleDirection)
        {
            this.target = target;
            this.attack = attack;
            this.stumbleDirection = stumbleDirection;
        }

        public Hitbox GetTarget() { return target; }

        public Attack GetAttack() { return attack; }

        public Vector3 GetStumbleDirection() { return stumbleDirection;}
    }
    private List<RegisteredHit> registeredHits = new List<RegisteredHit>();

    private void Awake()
    {
        instance = this;
        player1 = GameObject.Find("Player1").GetComponent<Player>();
        player2 = GameObject.Find("Player2").GetComponent<Player>();
        centerPos = GameObject.Find("PlayerCenterPosition").transform;
        startPlayerDistance = PlayerDistance();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFrame();
    }

    void CalculateOutput(Player player, PlayerInputData inputData)
    {
        // Record the player's inputs on the combo reader.
        ComboReader comboReader = player.GetComboReader();
        ComboInputData[] comboInputs = comboReader.inputs.ToArray();
        int comboInputsCount = comboInputs.Length;
        int recentIndex = comboReader.RecentIndex();

        if (inputData.PressingInputCount() > 0 && !inputData.pressingStart && !inputData.pressingSelect)
        {
            player.GetComboReader().inputs.Add(new ComboInputData(inputData, player.IsFacingRight()));
            player.StartResetComboReaderTimer();
        }

        if(comboInputs.Length > 0)
        {
            if (player.GetCurrentState() == Player.CurrentState.running)
            {
                if (comboInputs[recentIndex].inputDirection == ComboInputData.InputDirection.backward || comboInputs[recentIndex].inputDirection == ComboInputData.InputDirection.up || comboInputs[recentIndex].inputDirection == ComboInputData.InputDirection.down)
                    player.Idle();
            }
        }

        // Check to see if player is in a state that can transition to attack state.
        bool readyToAttack = false;
        switch (player.GetCurrentState())
        {
            case Player.CurrentState.idle: readyToAttack = true; break;
            case Player.CurrentState.running: readyToAttack = true; break;
        }

        // Execute combo moves if combo is successful.
        if (player.IsReadingCombos())
        {
            // Combo initiators.
            if (player.GetInputtedCombosCount() == 0 && readyToAttack)
            {
                foreach (ComboGraph.Branch branch in player.GetComboGraph().branches)
                {
                    if (branch.attack.MatchesRequiredInputs(comboReader.inputs))
                        player.ExecuteAttack(branch);
                }
            }
            else
            {
                // Follow up combos.
                int playerPerformedCombosCount = player.GetPerformedCombosList().Count;
                if (playerPerformedCombosCount > 0)
                {
                    foreach (ComboGraph.Branch branch in player.GetPerformedCombosList()[playerPerformedCombosCount - 1].followUpCombos)
                    {
                        if (branch.attack.MatchesRequiredInputs(comboReader.inputs))
                            player.ExecuteAttack(branch);
                    }
                }
            }
        }

        // Movement input while standing.
        if (player.GetCurrentState() == Player.CurrentState.idle)
        {
            if (player.IsFacingRight())
            {
                if (inputData.holdingLeft)
                    player.MoveBackward();
                else if (inputData.holdingRight)
                    player.MoveForward();
                else if (inputData.holdingUp)
                    player.SideStepLeft();
                else if (inputData.holdingDown)
                    player.SideStepRight();
            }
            else
            {
                if (inputData.holdingLeft)
                    player.MoveForward();
                else if (inputData.holdingRight)
                    player.MoveBackward();
                else if (inputData.holdingUp)
                    player.SideStepRight();
                else if (inputData.holdingDown)
                    player.SideStepLeft();
            }
        }

        // Movement input while laying.
        if (player.GetCurrentState() == Player.CurrentState.lay)
        {
            if (player.IsFacingRight())
            {
                if (inputData.holdingLeft)
                    player.RollBackward();
                else if (inputData.holdingRight)
                    player.RollForward();
            }
            else
            {
                if (inputData.holdingLeft)
                    player.RollForward();
                else if (inputData.holdingRight)
                    player.RollBackward();
            }
        }

        //  Determine if player should face it's opponent depending on it's current state.
        bool faceOpponent = false;
        switch (player.GetCurrentState())
        {
            case Player.CurrentState.idle: faceOpponent = true; break;
            case Player.CurrentState.running: faceOpponent = true; break;
        }

        if(faceOpponent)
            player.FaceOpponent();
    }

    // See what hits registered in the last frame and make them take effect.
    public void ApplyRegisteredHits()
    {
        foreach (RegisteredHit registeredHit in registeredHits)
        {
            Entity hitEntity = registeredHit.GetTarget().GetEntity();
            Hitbox hitbox = registeredHit.GetTarget();
            Attack attackData = registeredHit.GetAttack();

            // Check that hit target is a player.
            Player player = hitEntity as Player;
            if(player != null)
            {
                player.SetStumbleFrames(attackData.stumbleFrames);
                player.SetStumbleDirection(registeredHit.GetStumbleDirection());
                player.SetStumbleSpeed(attackData.stumbleSpeed);
                player.SetYVelocity(attackData.yVelocityLaunch);

                if (attackData.attackType == Attack.AttackType.stumble)
                    player.HighHit();
                else if(attackData.attackType == Attack.AttackType.launch)
                    player.LaunchHit();
            }
        }

        registeredHits.RemoveRange(0, registeredHits.Count);
    }

    Frame UpdateFrame()
    {
        Frame updateFrame = new Frame();
        updateFrame.player1Input = PlayerInputData.CloneData(InputReader.Player1());
        updateFrame.player2Input = PlayerInputData.CloneData(InputReader.Player2());

        ApplyRegisteredHits();
        CalculateOutput(player1, updateFrame.player1Input);
        CalculateOutput(player2, updateFrame.player2Input);
        player1.OnTick();
        player2.OnTick();

        // Position player center reference, and rotate for camera reference.
        centerPos.transform.position = (player1.Pos() + player2.Pos()) / 2f;

        if (!flipCamera)
            centerPos.LookAt(player1.Pos());
        else
            centerPos.LookAt(player2.Pos());

        centerPos.rotation = Quaternion.Euler(0f, centerPos.rotation.eulerAngles.y + 90f, 0f);

        // Add frame to frames list.
        currentFrame++;
        frames.Add(updateFrame);
        return updateFrame;
    }

    public void AddRegisteredHit(Hitbox target, Attack attack, Vector3 stumbleDirection)
    {
        registeredHits.Add(new RegisteredHit(target, attack, stumbleDirection));
    }

    public int GetCurrentFrame() {  return currentFrame; }
    public Player GetPlayer1() { return player1; }
    public Player GetPlayer2() { return player2; }
    public bool GetFlipCamera() {  return flipCamera; }
    public float PlayerDistance() { return Vector3.Distance(player1.Pos(), player2.Pos()); }
    public float GetMinPlayerDistance() {  return minPlayerDistance; }
    public float GetMaxPlayerDistance() { return maxPlayerDistance; }
    public float GetStartPlayerDistance() { return startPlayerDistance; }
}
