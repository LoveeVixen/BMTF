// LOVEEVIXEN
using UnityEngine;
using Fusion;
using EntitySystem;

namespace InputSystem
{
    public class PlayerController : NetworkBehaviour
    {
        [Range(1, 2)] private int localPlayerNumber = 1;
        private Player player;
        //private NetworkInputData receivedInputData;
        private bool isLocalPlayer = true;

        private void Awake()
        {
            player = GetComponent<Player>();
        }

        private void Start()
        {
            // Disable input control if this player doesn't belong to local client.
            if (gameObject.GetComponent<NetworkObject>().InputAuthority != NetworkManager.instance.GetRunner().LocalPlayer)
                isLocalPlayer = false;
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            // Get saved input from last network tick and output to player.
            if (GetInput(out NetworkInputData input) && SessionManager.instance.HasRoundBegun())
            {
                //receivedInputData = input;
                Output(input);
            }
        }

        void Output(NetworkInputData inputData)
        {
            // Record the player's inputs on the combo reader.
            //NetworkInputData inputData = receivedInputData;
            NetworkButtons pressedButtons = inputData.GetLocalPlayerInputs(localPlayerNumber);
            ComboReader comboReader = player.GetComboReader();
            ComboInputData[] comboInputs = comboReader.inputs.ToArray();
            int comboInputsCount = comboInputs.Length;
            int recentIndex = comboReader.RecentIndex();

            // Movement input while standing.
            if (player.GetCurrentState() == Player.CurrentState.idle)
            {
                if (player.IsFacingRight())
                {
                    if (pressedButtons.IsSet(NetworkInputHandler.Inputs.holdingLeft))
                        player.MoveBackward();
                    else if (pressedButtons.IsSet(NetworkInputHandler.Inputs.holdingRight))
                        player.MoveForward();
                    else if (pressedButtons.IsSet(NetworkInputHandler.Inputs.holdingUp))
                        player.SideStepLeft();
                    else if (pressedButtons.IsSet(NetworkInputHandler.Inputs.holdingDown))
                        player.SideStepRight();
                    else
                        player.StopMovement();
                }
                else
                {
                    if (pressedButtons.IsSet(NetworkInputHandler.Inputs.holdingLeft))
                        player.MoveForward();
                    else if (pressedButtons.IsSet(NetworkInputHandler.Inputs.holdingRight))
                        player.MoveBackward();
                    else if (pressedButtons.IsSet(NetworkInputHandler.Inputs.holdingUp))
                        player.SideStepRight();
                    else if (pressedButtons.IsSet(NetworkInputHandler.Inputs.holdingDown))
                        player.SideStepLeft();
                    else
                        player.StopMovement();
                }
            }

            // Movement input while laying.
            if (player.GetCurrentState() == Player.CurrentState.lay)
            {
                if (player.IsFacingRight())
                {
                    if (pressedButtons.IsSet(NetworkInputHandler.Inputs.holdingLeft))
                        player.RollBackward();
                    else if (pressedButtons.IsSet(NetworkInputHandler.Inputs.holdingRight))
                        player.RollForward();
                }
                else
                {
                    if (pressedButtons.IsSet(NetworkInputHandler.Inputs.holdingLeft))
                        player.RollForward();
                    else if (pressedButtons.IsSet(NetworkInputHandler.Inputs.holdingRight))
                        player.RollBackward();
                }
            }

            if (inputData.PressingInputCount(inputData.GetLocalPlayerInputs(localPlayerNumber)) > 0 && !pressedButtons.IsSet(NetworkInputHandler.Inputs.pressingStart) && !pressedButtons.IsSet(NetworkInputHandler.Inputs.pressingSelect))
            {
                player.GetComboReader().inputs.Add(new ComboInputData(pressedButtons, player.IsFacingRight()));
                player.StartResetComboReaderTimer();
            }

            if (comboInputs.Length > 0)
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

            //  Determine if player should face it's opponent depending on it's current state.
            bool faceOpponent = false;
            switch (player.GetCurrentState())
            {
                case Player.CurrentState.idle: faceOpponent = true; break;
                case Player.CurrentState.running: faceOpponent = true; break;
            }

            if (faceOpponent)
                player.FaceOpponent();
        }

        public NetworkButtons PlayerInputsOnTick(int localPlayer, NetworkInputData input)
        {
            if (localPlayer == 1)
                return input.player1Inputs;
            else if (localPlayer == 2)
                return input.player2Inputs;

            return default;
        }

        public int LocalPlayerNumber
        {
            get { return localPlayerNumber; }
            set { localPlayerNumber = value; }
        }
    }
}