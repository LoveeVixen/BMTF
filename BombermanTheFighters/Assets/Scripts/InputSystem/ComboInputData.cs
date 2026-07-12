// LOVEEVIXEN
using Fusion;
using UnityEngine;

namespace InputSystem
{
    [System.Serializable]
    public class ComboInputData
    {
        public enum InputDirection { none, up, upForward, forward, downForward, down, downBackward, backward, upBackward };
        public InputDirection inputDirection;
        public bool pressing0;
        public bool pressing1;
        public bool pressing2;
        public bool pressing3;

        /*public ComboInputData(PlayerInputData data, bool facingRight)
        {
            if (data.pressingUp)
                inputDirection = InputDirection.up;
            else if (data.pressingUpRight && facingRight || data.pressingUpLeft && !facingRight)
                inputDirection = InputDirection.upForward;
            else if (data.pressingRight && facingRight || data.pressingLeft && !facingRight)
                inputDirection = InputDirection.forward;
            else if (data.pressingDownRight && facingRight || data.pressingDownLeft && !facingRight)
                inputDirection = InputDirection.downForward;
            else if (data.pressingDown)
                inputDirection = InputDirection.down;
            else if (data.pressingDownLeft && facingRight || data.pressingDownRight && !facingRight)
                inputDirection = InputDirection.downBackward;
            else if (data.pressingLeft && facingRight || data.pressingRight && !facingRight)
                inputDirection = InputDirection.backward;
            else if (data.pressingUpLeft && facingRight || data.pressingUpRight && !facingRight)
                inputDirection = InputDirection.upBackward;

            pressing0 = data.pressing0;
            pressing1 = data.pressing1;
            pressing2 = data.pressing2;
            pressing3 = data.pressing3;
        }*/

        public ComboInputData(NetworkButtons pressedInputs, bool facingRight)
        {
            if (pressedInputs.IsSet(NetworkInputHandler.Inputs.pressingUp))
                inputDirection = InputDirection.up;
            else if (pressedInputs.IsSet(NetworkInputHandler.Inputs.pressingUpRight) && facingRight || pressedInputs.IsSet(NetworkInputHandler.Inputs.pressingUpLeft) && !facingRight)
                inputDirection = InputDirection.upForward;
            else if (pressedInputs.IsSet(NetworkInputHandler.Inputs.pressingRight) && facingRight || pressedInputs.IsSet(NetworkInputHandler.Inputs.pressingLeft) && !facingRight)
                inputDirection = InputDirection.forward;
            else if (pressedInputs.IsSet(NetworkInputHandler.Inputs.pressingDownRight) && facingRight || pressedInputs.IsSet(NetworkInputHandler.Inputs.pressingDownLeft) && !facingRight)
                inputDirection = InputDirection.downForward;
            else if (pressedInputs.IsSet(NetworkInputHandler.Inputs.pressingDown))
                inputDirection = InputDirection.down;
            else if (pressedInputs.IsSet(NetworkInputHandler.Inputs.pressingDownLeft) && facingRight || pressedInputs.IsSet(NetworkInputHandler.Inputs.pressingDownRight) && !facingRight)
                inputDirection = InputDirection.downBackward;
            else if (pressedInputs.IsSet(NetworkInputHandler.Inputs.pressingLeft) && facingRight || pressedInputs.IsSet(NetworkInputHandler.Inputs.pressingRight) && !facingRight)
                inputDirection = InputDirection.backward;
            else if (pressedInputs.IsSet(NetworkInputHandler.Inputs.pressingUpLeft) && facingRight || pressedInputs.IsSet(NetworkInputHandler.Inputs.pressingUpRight) && !facingRight)
                inputDirection = InputDirection.upBackward;

            pressing0 = pressedInputs.IsSet(NetworkInputHandler.Inputs.pressing0);
            pressing1 = pressedInputs.IsSet(NetworkInputHandler.Inputs.pressing1);
            pressing2 = pressedInputs.IsSet(NetworkInputHandler.Inputs.pressing2);
            pressing3 = pressedInputs.IsSet(NetworkInputHandler.Inputs.pressing3);
        }

        public ComboInputData() { }

        public bool MatchesRequiredInput(ComboInputData requiredInput)
        {
            bool matches = true;
            if(requiredInput.inputDirection != InputDirection.none)
            { 
                if(requiredInput.inputDirection != inputDirection)
                    matches = false;
            }

            if(requiredInput.pressing0 != pressing0)
                matches = false;

            if (requiredInput.pressing1 != pressing1)
                matches = false;

            if (requiredInput.pressing2 != pressing2)
                matches = false;

            if (requiredInput.pressing3 != pressing3)
                matches = false;

            /*if (requiredInput.pressing0 && !pressing0)
                matches = false;

            if (requiredInput.pressing1 && !pressing1)
                matches = false;

            if (requiredInput.pressing2 && !pressing2)
                matches = false;

            if (requiredInput.pressing3 && !pressing3)
                matches = false;*/

            return matches;
        }
    }
}