// LOVEEVIXEN
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Fusion;

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
            _ = NetworkManager.instance.HostOfflineSession();
        }

        public void HostRoom()
        {
            _ = NetworkManager.instance.HostSession();
        }

        public void JoinRoom(InputField roomNameInput)
        {
            _ = NetworkManager.instance.JoinSession(roomNameInput.text);
        }

        public void LeaveRoom()
        {
            _ = NetworkManager.instance.LeaveSession();
        }

        public void SetNickname(InputField nicknameInput)
        {
            NetworkManager.instance.NicknameOnJoin = nicknameInput.text;
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void BeginMatch()
        {
            NetworkManager.instance.GetRunner().LoadScene("Game");
        }

        public void ExitApplication()
        {
            Application.Quit();
        }

        public bool IsOpen() { return isOpen; }
    }
}