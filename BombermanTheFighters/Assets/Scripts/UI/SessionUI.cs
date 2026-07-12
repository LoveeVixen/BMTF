// LOVEEVIXEN
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SessionUI : MonoBehaviour
    {
        [SerializeField] Text mainAnnouncementText;
        [SerializeField] Text subAnnouncementText;
        [SerializeField] Sprite winCounterEmpty;
        [SerializeField] Sprite winCounterFill;

        public void DisplayAnnouncement(string announcement)
        {
            mainAnnouncementText.text = announcement;
            subAnnouncementText.text = "";
        }

        public void DisplayAnnouncement(string mainAnnouncement, string subAnnouncement)
        {
            mainAnnouncementText.text = mainAnnouncement;
            subAnnouncementText.text = subAnnouncement;
        }

        public void ClearAnnouncement()
        {
            mainAnnouncementText.text = "";
            subAnnouncementText.text = "";
        }
    }
}