// LOVEEVIXEN
using UnityEngine;

namespace ChatSystem
{
    public class ChatMessage
    {
        private string sender = "";
        private string message = "";
        private Color senderColor = Color.white;
        private Color messageColor = Color.white;

        public ChatMessage(string sender, string message)
        {
            this.sender = sender;
            this.message = message;
        }

        public ChatMessage(string sender, string message, Color senderColor)
        {
            this.sender = sender;
            this.message = message;
            this.senderColor = senderColor;
        }

        public ChatMessage(string sender, string message, Color senderColor, Color messageColor)
        {
            this.sender = sender;
            this.message = message;
            this.senderColor = senderColor;
            this.messageColor = messageColor;
        }

        public string GetSender() {  return sender; }
        public string GetMessage() { return message; }
        public Color GetSenderColor() { return senderColor; }
        public Color GetMessageColor() {  return messageColor; }
    }
}