// LOVEEVIXEN
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using UnityEngine;

namespace InputSystem
{
    public class NetworkInputHandler : MonoBehaviour, INetworkRunnerCallbacks
    {
        // Keep both arrays on same length value as they work together.
        private NetworkButtons[] localInputs = new NetworkButtons[2];

        public enum Inputs
        {
            // Holding input.
            holdingUp, // 0
            holdingRight, // 1
            holdingDown, // 2
            holdingLeft, // 3
            holdingUpRight, // 4
            holdingDownRight, // 5
            holdingDownLeft, // 6
            holdingUpLeft, // 7
            holding0, // 8
            holding1, // 9
            holding2, // 10
            holding3, // 11
            holdingStart, // 12
            holdingSelect, // 13

            // Press/tap input.
            pressingUp, // 14
            pressingRight, // 15
            pressingDown, // 16
            pressingLeft, // 17
            pressingUpRight, // 18
            pressingDownRight, // 19
            pressingDownLeft, // 20
            pressingUpLeft, // 21
            pressing0, // 22
            pressing1, // 23
            pressing2, // 24
            pressing3, // 25
            pressingStart, // 26
            pressingSelect // 27
        }

        void Start()
        {
            NetworkManager.instance.GetRunner().AddCallbacks(this);
        }

        /*void Update()
        {
            for(int i = 0; i < InputReader.AllPlayersInputData().Length; i++)
            {
                PlayerInputData localInputData = InputReader.AllPlayersInputData()[i];

                localInputs[i].Set(Inputs.holdingUp, localInputData.holdingUp);
                localInputs[i].Set(Inputs.holdingRight, localInputData.holdingRight);
                localInputs[i].Set(Inputs.holdingDown, localInputData.holdingDown);
                localInputs[i].Set(Inputs.holdingLeft, localInputData.holdingLeft);
                localInputs[i].Set(Inputs.holdingUpRight, localInputData.holdingUpRight);
                localInputs[i].Set(Inputs.holdingDownRight, localInputData.holdingDownRight);
                localInputs[i].Set(Inputs.holdingDownLeft, localInputData.holdingDownLeft);
                localInputs[i].Set(Inputs.holdingUpLeft, localInputData.holdingUpLeft);
                localInputs[i].Set(Inputs.holding0, localInputData.holding0);
                localInputs[i].Set(Inputs.holding1, localInputData.holding1);
                localInputs[i].Set(Inputs.holding2, localInputData.holding2);
                localInputs[i].Set(Inputs.holding3, localInputData.holding3);
                localInputs[i].Set(Inputs.holdingStart, localInputData.holdingStart);
                localInputs[i].Set(Inputs.holdingSelect, localInputData.holdingSelect);

                if (localInputData.pressingUp) localInputs[i].Set(Inputs.pressingUp, true);
                if (localInputData.pressingRight) localInputs[i].Set(Inputs.pressingRight, true);
                if (localInputData.pressingDown) localInputs[i].Set(Inputs.pressingDown, true);
                if (localInputData.pressingLeft) localInputs[i].Set(Inputs.pressingLeft, true);
                if (localInputData.pressingUpRight) localInputs[i].Set(Inputs.pressingUpRight, true);
                if (localInputData.pressingDownRight) localInputs[i].Set(Inputs.pressingDownRight, true);
                if (localInputData.pressingDownLeft) localInputs[i].Set(Inputs.pressingDownLeft, true);
                if (localInputData.pressingUpLeft) localInputs[i].Set(Inputs.pressingUpLeft, true);
                if (localInputData.pressing0) localInputs[i].Set(Inputs.pressing0, true);
                if (localInputData.pressing1) localInputs[i].Set(Inputs.pressing1, true);
                if (localInputData.pressing2) localInputs[i].Set(Inputs.pressing2, true);
                if (localInputData.pressing3) localInputs[i].Set(Inputs.pressing3, true);
                if (localInputData.pressingStart) localInputs[i].Set(Inputs.pressingStart, true);
                if (localInputData.pressingSelect) localInputs[i].Set(Inputs.pressingSelect, true);
            }
        }*/

        void OnDestroy()
        {
            NetworkManager.instance.GetRunner().RemoveCallbacks(this);
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

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            for (int i = 0; i < InputReader.AllPlayersInputData().Length; i++)
            {
                PlayerInputData localInputData = InputReader.AllPlayersInputData()[i];

                localInputs[i].Set(Inputs.holdingUp, localInputData.holdingUp);
                localInputs[i].Set(Inputs.holdingRight, localInputData.holdingRight);
                localInputs[i].Set(Inputs.holdingDown, localInputData.holdingDown);
                localInputs[i].Set(Inputs.holdingLeft, localInputData.holdingLeft);
                localInputs[i].Set(Inputs.holdingUpRight, localInputData.holdingUpRight);
                localInputs[i].Set(Inputs.holdingDownRight, localInputData.holdingDownRight);
                localInputs[i].Set(Inputs.holdingDownLeft, localInputData.holdingDownLeft);
                localInputs[i].Set(Inputs.holdingUpLeft, localInputData.holdingUpLeft);
                localInputs[i].Set(Inputs.holding0, localInputData.holding0);
                localInputs[i].Set(Inputs.holding1, localInputData.holding1);
                localInputs[i].Set(Inputs.holding2, localInputData.holding2);
                localInputs[i].Set(Inputs.holding3, localInputData.holding3);
                localInputs[i].Set(Inputs.holdingStart, localInputData.holdingStart);
                localInputs[i].Set(Inputs.holdingSelect, localInputData.holdingSelect);

                if (localInputData.pressingUp) localInputs[i].Set(Inputs.pressingUp, true);
                if (localInputData.pressingRight) localInputs[i].Set(Inputs.pressingRight, true);
                if (localInputData.pressingDown) localInputs[i].Set(Inputs.pressingDown, true);
                if (localInputData.pressingLeft) localInputs[i].Set(Inputs.pressingLeft, true);
                if (localInputData.pressingUpRight) localInputs[i].Set(Inputs.pressingUpRight, true);
                if (localInputData.pressingDownRight) localInputs[i].Set(Inputs.pressingDownRight, true);
                if (localInputData.pressingDownLeft) localInputs[i].Set(Inputs.pressingDownLeft, true);
                if (localInputData.pressingUpLeft) localInputs[i].Set(Inputs.pressingUpLeft, true);
                if (localInputData.pressing0) localInputs[i].Set(Inputs.pressing0, true);
                if (localInputData.pressing1) localInputs[i].Set(Inputs.pressing1, true);
                if (localInputData.pressing2) localInputs[i].Set(Inputs.pressing2, true);
                if (localInputData.pressing3) localInputs[i].Set(Inputs.pressing3, true);
                if (localInputData.pressingStart) localInputs[i].Set(Inputs.pressingStart, true);
                if (localInputData.pressingSelect) localInputs[i].Set(Inputs.pressingSelect, true);
            }

            NetworkInputData networkInputData = new NetworkInputData();
            networkInputData.player1Inputs = localInputs[0];
            networkInputData.player2Inputs = localInputs[1];

            // Send to network.
            input.Set(networkInputData);

            // Reset inputs to make sure they don't repeat.
            for (int i = 0; i < localInputs.Length; i++)
                localInputs[i] = default;
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

        public void OnConnectedToServer(NetworkRunner runner) { }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

        public void OnSceneLoadDone(NetworkRunner runner) { }

        public void OnSceneLoadStart(NetworkRunner runner) { }

    }
}