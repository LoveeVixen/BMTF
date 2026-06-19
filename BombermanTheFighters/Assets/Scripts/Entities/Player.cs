// LOVEEVIXEN
using InputSystem;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    private const float moveSpeed = 0.15f;
    private Player opponent;

    // Animation
    private Animator anim;

    // Player state
    public enum CurrentState { idle, running, dashForward, dashBackward, dashLeft, dashRight, attacking, lay, knockout };
    private CurrentState currentState;

    // Run and dash settings
    private const float runSpeed = 0.6f;
    private const float dashSpeed = 0.5f;

    // Combos
    [SerializeField] ComboReader comboReader = new ComboReader();
    [SerializeField] ComboChart comboChart = new ComboChart();
    private int resetComboReaderEachSet = 20;
    private int framesUntilResetComboReader;
    [SerializeField] List<Attack> performedCombos = new List<Attack>();
    private bool readCombos = true;
    private int inputtedCombosCount = 0;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        if(this == SessionManager.instance.GetPlayer1())
            opponent = SessionManager.instance.GetPlayer2();
        else
            opponent = SessionManager.instance.GetPlayer1();
    }

    private void Update()
    {
        if(currentState == CurrentState.running)
        {
            // Make player run to it's opponent until close enough.
            if (SessionManager.instance.PlayerDistance() > SessionManager.instance.GetMinPlayerDistance())
                MoveDirection(transform.forward * runSpeed);
            else
                Idle();
        }
        /*else if(dashFrames > 0)
        {
            if (currentState == CurrentState.dashForward)
            {
                dashFrames--;
                if (SessionManager.instance.PlayerDistance() > SessionManager.instance.GetMinPlayerDistance())
                    MoveDirection(transform.forward * dashSpeed);
            }

            // Player dashes backwards for given amount of frames. Also prevents dashing past the distance limit.
            if (currentState == CurrentState.dashBackward)
            {
                dashFrames--;
                if (SessionManager.instance.PlayerDistance() < SessionManager.instance.GetMaxPlayerDistance())
                    MoveDirection(-transform.forward * dashSpeed);
            }

            // Player dashes left for a given amount of frames.
            if (currentState == CurrentState.dashLeft)
            {
                dashFrames--;
                MoveDirection(-transform.right * dashSpeed);
            }

            // Player dashes right for a given amount of frames.
            if (currentState == CurrentState.dashRight)
            {
                dashFrames--;
                MoveDirection(transform.right * dashSpeed);
            }

            if (dashFrames == 0)
            {
                // If dash was in forward direction, transition to running. Otherwise go back to idle.
                if (currentState == CurrentState.dashForward)
                    Run();
                else
                    Idle();
            }
        }*/

        if (currentState == CurrentState.dashForward)
        {
            if (SessionManager.instance.PlayerDistance() > SessionManager.instance.GetMinPlayerDistance())
                MoveDirection(transform.forward * dashSpeed);
        }

        if (currentState == CurrentState.dashBackward)
        {
            if (SessionManager.instance.PlayerDistance() < SessionManager.instance.GetMaxPlayerDistance())
                MoveDirection(-transform.forward * dashSpeed);
        }

        if (currentState == CurrentState.dashLeft)
            MoveDirection(-transform.right * dashSpeed);

        if (currentState == CurrentState.dashRight)
            MoveDirection(transform.right * dashSpeed);

        if (framesUntilResetComboReader > 0 && currentState == CurrentState.idle)
        {
            framesUntilResetComboReader--;
            if (framesUntilResetComboReader == 0 && currentState != CurrentState.attacking)
                ResetComboSystem();
        }
    }

    void MoveDirection(Vector3 direction)
    {
        transform.position += direction;
        SnapPosition();
    }

    public void Idle()
    {
        currentState = CurrentState.idle;
        PlayAnimation("Idle");
        ResetComboSystem();
    }

    public List<Attack> GetPerformedCombos()
    {
        return performedCombos;
    }

    public Attack LastPerformedCombo()
    {
        return performedCombos[performedCombos.Count - 1];
    }

    public void MoveForward()
    {
        if (SessionManager.instance.PlayerDistance() > SessionManager.instance.GetMinPlayerDistance())
            MoveDirection(transform.forward * moveSpeed);
    }

    public void MoveBackward()
    {
        if (SessionManager.instance.PlayerDistance() < SessionManager.instance.GetMaxPlayerDistance())
            MoveDirection(-transform.forward * moveSpeed);
    }

    public void SideStepRight()
    {
        MoveDirection(transform.right * moveSpeed);
    }

    public void SideStepLeft()
    {
        MoveDirection(-transform.right * moveSpeed);
    }

    public void FaceOpponent()
    {
        transform.LookAt(opponent.Pos());
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }

    public void StartResetComboReaderTimer()
    {
        framesUntilResetComboReader = resetComboReaderEachSet;
    }

    public void ExecuteAttack(Attack attack)
    {
        if (!attack.isGapInput)
        {
            readCombos = false;
            if (attack.playAnimation != "" && attack.playAnimation != "NOCLIP")
                PlayAnimation(attack.playAnimation);

            currentState = CurrentState.attacking;
        }

        performedCombos.Add(attack);
        inputtedCombosCount++;
    }

    public void PlayAnimation(string animName)
    {
        anim.Play(animName);
    }

    public bool IsFacingRight()
    {
        bool facingRight = false;

        if(this == SessionManager.instance.GetPlayer1() && !SessionManager.instance.GetFlipCamera())
            facingRight = true;
        else if(this == SessionManager.instance.GetPlayer1())
            facingRight = false;

        if (this == SessionManager.instance.GetPlayer2() && SessionManager.instance.GetFlipCamera())
            facingRight = true;
        else if(this == SessionManager.instance.GetPlayer2())
            facingRight = false;

        return facingRight;
    }

    public void ResetComboSystem()
    {
        comboReader.Reset();
        inputtedCombosCount = 0;
        ResetPerformedCombosList();
        readCombos = true;
    }

    public CurrentState GetCurrentState()
    {
        return currentState;
    }

    public ComboReader GetComboReader()
    {
        return comboReader;
    }

    public ComboChart GetComboChart()
    {
        return comboChart;
    }

    public void ReadCombos()
    {
        readCombos = true;
    }

    public bool IsReadingCombos()
    {
        return readCombos;
    }

    public int GetInputtedCombosCount()
    {
        return inputtedCombosCount;
    }

    void ResetPerformedCombosList()
    {
        performedCombos.RemoveRange(0, performedCombos.Count);
    }

    // Universal character attacks/abilities
    #region
    public void Run()
    {
        PlayAnimation("Run");
        currentState = CurrentState.running;
        ResetComboSystem();
    }

    public void DashForward()
    {
        currentState = CurrentState.dashForward;
    }

    public void DashBackward()
    {
        currentState = CurrentState.dashBackward;
    }

    public void DashLeft()
    {
        PlayAnimation("DashLeft");
        currentState = CurrentState.dashLeft;
    }

    public void DashRight()
    {
        PlayAnimation("DashRight");
        currentState = CurrentState.dashRight;
    }

    public void SideDash()
    {
        if (comboReader.inputs[comboReader.RecentIndex()].inputDirection == ComboInputData.InputDirection.up)
        {
            if (IsFacingRight())
                DashLeft();
            else
                DashRight();
        }
        else if (comboReader.inputs[comboReader.RecentIndex()].inputDirection == ComboInputData.InputDirection.down)
        {
            if (IsFacingRight())
                DashRight();
            else
                DashLeft();
        }
    }

    #endregion
}
