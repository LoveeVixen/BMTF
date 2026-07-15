// LOVEEVIXEN
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NetworkSessionLobby : MonoBehaviourPunCallbacks
    {
        public static NetworkSessionLobby instance;

        private Menu menu;
        [SerializeField] Text sessionName;
        [SerializeField] Text leftClientNickname;
        [SerializeField] Text rightClientNickname;
        [SerializeField] Button startMatchButton;
        [SerializeField] Button[] hostOnlyButtons;

        private void Awake()
        {
            instance = this;
            menu = GetComponent<Menu>();
        }

        private void Update()
        {
            UpdatePlayerListDisplay();
        }

        private void FixedUpdate()
        {
            if (menu.IsOpen() && PhotonNetwork.InRoom)
            {
                // Disable/enable host-only buttons depending on if this client is the host or not.
                bool isServer = PhotonNetwork.IsMasterClient;
                foreach (Button button in hostOnlyButtons)
                    button.interactable = isServer;

                // Prevent starting match until there are at least two clients present in room.
                bool canStartMatch = PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1;
                startMatchButton.interactable = canStartMatch;
            }
        }

        public void UpdateSessionNameText()
        {
            sessionName.text = PhotonNetwork.CurrentRoom.Name;
        }

        void UpdatePlayerListDisplay()
        {
            // Display nicknames of users currently present in the session.
            string[] names = new string[2];
            for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (names.Length > i)
                    names[i] = PhotonNetwork.PlayerList[i].NickName;
            }

            leftClientNickname.text = names[0];
            rightClientNickname.text = names[1];
        }

        public Menu GetMenu()
        {
            return menu;
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            UpdateSessionNameText();
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            UpdateSessionNameText();
        }
    }
}