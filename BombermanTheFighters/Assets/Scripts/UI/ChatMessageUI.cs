// LOVEEVIXEN
using ChatSystem;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessageUI : MonoBehaviour
{
    [SerializeField] Text senderText;
    [SerializeField] Text messageText;

    public void ShowMessage(ChatMessage msg)
    {
        senderText.text = msg.GetSender();
        senderText.color = msg.GetSenderColor();
        messageText.text = msg.GetMessage();
        messageText.color = msg.GetMessageColor();
    }
}
