// LOVEEVIXEN
using Audio;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NetworkFailurePrompt : MonoBehaviour
    {
        public static NetworkFailurePrompt instance;

        private Menu menu;
        [SerializeField] Text headerText;
        [SerializeField] Text reasonText;

        private void Awake()
        {
            instance = this;
            menu = GetComponent<Menu>();
        }

        public void DisplayFailure(string subject, string reason)
        {
            headerText.text = subject;
            reasonText.text = reason;
        }

        public Menu GetMenu()
        {
            return menu;
        }
    }
}