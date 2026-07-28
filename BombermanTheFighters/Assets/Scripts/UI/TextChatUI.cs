// LOVEEVIXEN
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ChatSystem;

namespace UI
{
    public class TextChatUI : MonoBehaviour
    {
        public static TextChatUI instance;

        private bool open;
        private Transform container;
        [SerializeField] InputField chatInput;

        // Chat history.
        [SerializeField] Transform chatHistoryContainer;
        [SerializeField] GameObject chatMessagePrefab;
        private List<ChatMessageUI> messageDisplays = new List<ChatMessageUI>();
        private int maxAmountOfChatMessages = 20;
        private float alpha;
        [SerializeField] float fadeOutChatHistoryTime = 5f;
        private float fadeOutChatHistoryTimer = 0f;
        [Range(0.1f, 2f)][SerializeField] float fadeOutSpeed = 0.5f;
        private CanvasGroup chatHistoryAlphaControl;

        private void Awake()
        {
            instance = this;
            container = transform.GetChild(0);
            chatHistoryAlphaControl = GetComponentInChildren<CanvasGroup>();

            for(int i = 0; i < maxAmountOfChatMessages; i++)
            {
                GameObject chatMessageObj = Instantiate(chatMessagePrefab, chatHistoryContainer);
                messageDisplays.Add(chatMessageObj.GetComponent<ChatMessageUI>());
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Input functionality.
            if (Input.GetKeyDown(TextChat.chatInput) && !open)
                Open();

            if (Input.GetKeyDown(TextChat.closeInput) && open)
                Close();

            // Tick down timer until chat history is ready to fade out.
            if (fadeOutChatHistoryTimer > 0f)
            {
                fadeOutChatHistoryTimer -= Time.deltaTime;
                if (fadeOutChatHistoryTimer < 0f)
                    fadeOutChatHistoryTimer = 0f;
            }

            // Fade out chat history.
            if(fadeOutChatHistoryTimer == 0f && alpha > 0f)
            {
                alpha -= Time.deltaTime * fadeOutSpeed;
                if(alpha < 0f)
                    alpha = 0f;
            }

            // Apply calculated alpha value to chat history, unless chat is open.
            if (open)
                chatHistoryAlphaControl.alpha = 1f;
            else
                chatHistoryAlphaControl.alpha = alpha;
        }

        public void Open()
        {
            if (TextChat.IsConnectedToSession())
            {
                open = true;
                chatInput.text = TextChat.SavedMessage;
                chatInput.gameObject.SetActive(true);
                chatInput.Select();
            }
        }

        public void Close()
        {
            open = false;
            chatInput.gameObject.SetActive(false);
        }

        public void SendChatMessage()
        {
            if (chatInput.text != "" && TextChat.IsConnectedToSession())
            {
                Close();
                TextChat.SendChatMessage(chatInput.text);

                Menu currentlyOpenMenu = Menu.FindCurrentlyOpenMenu();
                if (currentlyOpenMenu != null)
                    currentlyOpenMenu.GetSelectButtonOnOpen().Select();
                else
                    EventSystem.current.SetSelectedGameObject(null);
            }
        }

        // Show all received messages on UI.
        public void UpdateChatHistory()
        {
            int showMessageIndex = TextChat.messages.Count - 1;
            for(int i = 0; i < messageDisplays.Count; i++)
            {
                if (showMessageIndex >= 0)
                {
                    ChatMessage msg = TextChat.messages[showMessageIndex];
                    messageDisplays[i].ShowMessage(msg);
                    showMessageIndex--;
                }
                else
                    break;
            }
        }

        public void SaveTypedMessage()
        {
            TextChat.SavedMessage = chatInput.text;
        }

        public void ResetChatHistoryFade()
        {
            alpha = 1f;
            fadeOutChatHistoryTimer = fadeOutChatHistoryTime;
        }

        public bool IsOpen() { return open; }
    }
}