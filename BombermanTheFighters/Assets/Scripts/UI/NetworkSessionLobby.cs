// LOVEEVIXEN
using Fusion;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NetworkSessionLobby : MonoBehaviour
    {
        public static NetworkSessionLobby instance;

        private Menu menu;
        [SerializeField] Text sessionName;
        [SerializeField] Text leftClientNickname;
        [SerializeField] Text rightClientNickname;
        [SerializeField] Button startMatchButton;
        [SerializeField] Button[] hostOnlyButtons;
        private float updatePlayerListTime = 0.5f;
        private float updatePlayerListTimer;

        private void Awake()
        {
            instance = this;
            menu = GetComponent<Menu>();
            updatePlayerListTimer = updatePlayerListTime;
        }

        private void Update()
        {
            if(updatePlayerListTimer > 0f)
            {
                updatePlayerListTimer -= Time.deltaTime;
                if(updatePlayerListTimer <= 0f)
                {
                    updatePlayerListTimer = updatePlayerListTime;
                    UpdatePlayerListDisplay();
                }
            }
        }

        private void FixedUpdate()
        {
            if (menu.IsOpen() && NetworkManager.instance.GetRunner() != null)
            {
                // Disable/enable host-only buttons depending on if this client is the host or not.
                bool isServer = NetworkManager.instance.GetRunner().IsServer;
                foreach (Button button in hostOnlyButtons)
                    button.interactable = isServer;

                // Prevent starting match until there are at least two clients present in room.
                bool canStartMatch = NetworkManager.instance.GetRunner().IsServer && NetworkManager.instance.GetRunner().SessionInfo.PlayerCount > 1;
                startMatchButton.interactable = canStartMatch;
            }
        }

        public void UpdateSessionNameText()
        {
            sessionName.text = NetworkManager.instance.GetRunner().SessionInfo.Name;
        }

        void UpdatePlayerListDisplay()
        {
            // Display nicknames of users currently present in the session.
            NetworkDataSync[] dataSyncs = FindObjectsByType<NetworkDataSync>(FindObjectsSortMode.InstanceID);

            string leftName = "";
            string rightName = "";
            foreach (NetworkDataSync dataSync in dataSyncs)
            {
                if(dataSync.startFacingRight)
                    leftName = dataSync.nickname.ToString();
                else
                    rightName = dataSync.nickname.ToString();
            }

            leftClientNickname.text = leftName;
            rightClientNickname.text = rightName;
        }

        public Menu GetMenu()
        {
            return menu;
        }
    }
}