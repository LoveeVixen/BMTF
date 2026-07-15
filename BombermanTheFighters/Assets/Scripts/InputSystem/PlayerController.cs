// LOVEEVIXEN
using UnityEngine;
using EntitySystem;

namespace InputSystem
{
    public class PlayerController : MonoBehaviour
    {
        private Player player;
        private PlayerInputData readInput;

        private void Awake()
        {
            player = GetComponent<Player>();
        }

        private void Start()
        {
            // Disable player control on this player character for anyone who isn't the local client.
            if(!player.photonView.IsMine)
                enabled = false;
        }

        private void Update()
        {
            if (SessionManager.instance.HasRoundBegun())
            {
                player.OutputInputData(PlayerInputData.CloneData(readInput));
                player.Output();
            }
        }

        public void ReadFromInputData(PlayerInputData setReadInput)
        {
            readInput = setReadInput;
        }
    }
}