// LOVEEVIXEN
using Fusion;
using System;

namespace InputSystem
{
    public struct NetworkInputData : INetworkInput
    {
        public NetworkButtons player1Inputs;
        public NetworkButtons player2Inputs;

        public int PressingInputCount(NetworkButtons localPlayerInputs)
        {
            int countedInputs = 0;
            int totalGameInputs = Enum.GetValues(typeof(NetworkInputHandler.Inputs)).Length;

            // Go over all pressing inputs. Start i off at 14 to ignore holding inputs.
            for(int i = 14; i < totalGameInputs; i++)
            {
                if (localPlayerInputs.IsSet(i))
                    countedInputs++;
            }

            return countedInputs;
        }

        public NetworkButtons GetLocalPlayerInputs(int localPlayerNumber)
        {
            if (localPlayerNumber == 1)
                return player1Inputs;
            else if(localPlayerNumber == 2)
                return player2Inputs;

            return default;
        }
    }
}