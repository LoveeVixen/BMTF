// LOVEEVIXEN
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class Menu : MonoBehaviour
    {
        private bool isOpen = true;
        private GameObject container;
        [SerializeField] Button selectButtonOnOpen;

        private void Awake()
        {
            container = transform.GetChild(0).gameObject;
        }

        public void Open()
        {
            // Close any other menu before opening this menu.
            Menu[] menus = FindObjectsByType<Menu>(FindObjectsSortMode.None);
            foreach(Menu menu in menus)
            {
                if (menu != this)
                    menu.Close();
            }

            // Open menu, highlight menu's default button.
            container.SetActive(true);
            isOpen = true;

            if(selectButtonOnOpen != null)
                selectButtonOnOpen.Select();
        }

        public void Close()
        {
            container.SetActive(false);
            isOpen = false;
        }

        public void OfflineGame()
        {
            NetworkManager.instance.HostOfflineSession();
        }

        public void HostRoom()
        {
            NetworkManager.instance.HostSession();
        }

        public void JoinRoom(InputField roomNameInput)
        {
            NetworkManager.instance.JoinSession(roomNameInput.text);
        }

        public void LeaveRoom()
        {
            NetworkManager.instance.LeaveSession();
        }

        public void DisplayNicknameOnInputField(InputField nicknameInput)
        {
            nicknameInput.text = PhotonNetwork.NickName;
        }

        public void SetNickname(InputField nicknameInput)
        {
            if (nicknameInput.text != "")
                NetworkManager.instance.SetNickname(nicknameInput.text);
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void BeginMatch()
        {
            NetworkClient networkClient = FindFirstObjectByType<NetworkClient>();
            networkClient.photonView.RPC("BeginMatch", RpcTarget.All);
        }

        public void DisconnectFromServer()
        {
            PhotonNetwork.Disconnect();
        }

        public void ExitApplication()
        {
            Application.Quit();
        }

        public bool IsOpen() { return isOpen; }
        public Button GetSelectButtonOnOpen() {  return selectButtonOnOpen; }

        // Find which menu is currently open.
        public static Menu FindCurrentlyOpenMenu()
        {
            Menu[] menus = FindObjectsByType<Menu>(FindObjectsSortMode.None);
            foreach(Menu menu in menus)
            {
                if (menu.IsOpen())
                    return menu;
            }

            return null;
        }
    }
}