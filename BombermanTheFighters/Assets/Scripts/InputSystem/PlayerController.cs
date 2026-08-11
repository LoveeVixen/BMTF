// LOVEEVIXEN
using UnityEngine;
using System.Collections.Generic;
using EntitySystem;
using UI;

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
                PlayerInputData outputInputData = new PlayerInputData();

                if (FeedInput() && readInput != null)
                    outputInputData = PlayerInputData.CloneData(readInput);
                
                player.OutputInputData(outputInputData);
                player.Output();
            }
        }

        public void ReadFromInputData(PlayerInputData setReadInput)
        {
            readInput = setReadInput;
        }

        public bool FeedInput()
        {
            bool feedInput = true;
            if(TextChatUI.instance.IsOpen())
                feedInput = false;

            return feedInput;
        }
    }
}