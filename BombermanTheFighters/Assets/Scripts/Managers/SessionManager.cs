// LOVEEVIXEN
using UnityEngine;
using System.Collections.Generic;
using InputSystem;
using EntitySystem;

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

        // Move player in inputted direction.
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

    Frame UpdateFrame()
    {
        Frame updateFrame = new Frame();
        updateFrame.player1Input = PlayerInputData.CloneData(InputReader.Player1());
        updateFrame.player2Input = PlayerInputData.CloneData(InputReader.Player2());

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

    public int GetCurrentFrame() {  return currentFrame; }
    public Player GetPlayer1() { return player1; }
    public Player GetPlayer2() { return player2; }
    public bool GetFlipCamera() {  return flipCamera; }
    public float PlayerDistance() { return Vector3.Distance(player1.Pos(), player2.Pos()); }
    public float GetMinPlayerDistance() {  return minPlayerDistance; }
    public float GetMaxPlayerDistance() { return maxPlayerDistance; }
    public float GetStartPlayerDistance() { return startPlayerDistance; }
}
