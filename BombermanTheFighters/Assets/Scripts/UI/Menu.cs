// LOVEEVIXEN
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

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void ExitApplication()
        {
            Application.Quit();
        }

        public bool IsOpen() { return isOpen; }
    }
}