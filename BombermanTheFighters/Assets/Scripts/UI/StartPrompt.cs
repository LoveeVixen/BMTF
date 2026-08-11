// LOVEEVIXEN
using Audio;
using InputSystem;
using UnityEngine;

namespace UI
{
    public class StartPrompt : MonoBehaviour
    {
        private bool isOpen = true;
        private GameObject container;
        [SerializeField] Menu openMenuOnStart;
        [SerializeField] string playMusic;

        private void Awake()
        {
            container = transform.GetChild(0).gameObject;
        }

        private void Start()
        {
            GameManager.instance.SetLocalPlayersCanJoin(true);
            AudioManager.instance.PlayMusic(playMusic);
        }

        // Update is called once per frame
        void Update()
        {
            // Check that game has yet to be initiated.
            if (isOpen)
            {
                // Close start prompt once someone is playing.
                for(int i = 0; i < InputReader.AllPlayersInputData().Length; i++)
                {
                    if (GameManager.instance.IsPlaying()[i])
                    {
                        Close();
                        openMenuOnStart.Open();
                    }
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