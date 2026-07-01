// LOVEEVIXEN
using InputSystem;
using UnityEngine;

namespace UI
{
    public class StartPrompt : MonoBehaviour
    {
        private bool isOpen = true;
        private GameObject container;
        [SerializeField] Menu openMenuOnStart;

        private void Awake()
        {
            container = transform.GetChild(0).gameObject;
        }

        // Update is called once per frame
        void Update()
        {
            // Check that game has yet to be initiated.
            if (isOpen)
            {
                // Player 1 initiates game.
                if (InputReader.Player1().pressingStart)
                {
                    Close();
                    openMenuOnStart.Open();
                }

                // Player 2 initiates game.
                if (InputReader.Player2().pressingStart)
                {
                    Close();
                    openMenuOnStart.Open();
                }
            }
        }

        public void Open()
        {
            container.SetActive(true);
            isOpen = true;
        }

        public void Close()
        {
            container.SetActive(false);
            isOpen = false;
        }

        public bool IsOpen()
        {
            return isOpen;
        }
    }
}