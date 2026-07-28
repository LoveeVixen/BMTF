// LOVEEVIXEN
using EntitySystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SessionUI : MonoBehaviour
    {
        private Player leftPlayer;
        private Player rightPlayer;

        [SerializeField] Text mainAnnouncementText;
        [SerializeField] Text subAnnouncementText;
        [SerializeField] Text timer;
        [SerializeField] Text leftPlayerNameText;
        [SerializeField] Text rightPlayerNameText;

        [Header("Health Display")]
        [Range(0.01f, 20f)][SerializeField] float healthSliderLerpSpeed = 5f;
        private float leftPlayerHealth = 0f;
        private float leftPlayerLastHealth = 0f;
        private float pauseLeftDamageSliderTimer = 0f;
        [SerializeField] Slider leftPlayerHealthSlider;
        [SerializeField] Slider leftPlayerDamageSlider;
        private float rightPlayerHealth = 0f;
        private float rightPlayerLastHealth = 0f;
        private float pauseRightDamageSliderTimer = 0f;
        [SerializeField] Slider rightPlayerHealthSlider;
        [SerializeField] Slider rightPlayerDamageSlider;

        [Header("Win Counter Display")]
        [SerializeField] Sprite winCounterEmpty;
        [SerializeField] Sprite winCounterFill;
        [SerializeField] Image[] leftPlayerWinCounters = new Image[5];
        [SerializeField] Image[] rightPlayerWinCounters = new Image[5];

        private void Start()
        {
            for (int i = 0; i < leftPlayerWinCounters.Length; i++)
                leftPlayerWinCounters[i].gameObject.SetActive(Gamerules.usingGamerules.rounds > i);

            for (int i = 0; i < rightPlayerWinCounters.Length; i++)
                rightPlayerWinCounters[i].gameObject.SetActive(Gamerules.usingGamerules.rounds > i);
        }

        private void Update()
        {
            // Display time left of the round.
            string timeLeft = SessionManager.instance.GetTimer().ToString("F0");
            if (SessionManager.instance.GetTimer() <= 10f)
                timeLeft = SessionManager.instance.GetTimer().ToString("F2");

            timer.text = timeLeft;

            // Pause damage sliders when players have recently taken damage.
            if (pauseLeftDamageSliderTimer > 0f)
            {
                pauseLeftDamageSliderTimer -= Time.deltaTime;
                if(pauseLeftDamageSliderTimer < 0f)
                    pauseLeftDamageSliderTimer = 0f;
            }

            if (pauseRightDamageSliderTimer > 0f)
            {
                pauseRightDamageSliderTimer -= Time.deltaTime;
                if (pauseRightDamageSliderTimer < 0f)
                    pauseRightDamageSliderTimer = 0f;
            }

            // Show left player health/damage.
            if (leftPlayer != null)
            {
                leftPlayerHealth = leftPlayer.GetHealth().CurrentHealth;
                if (leftPlayerHealth != leftPlayerLastHealth) pauseLeftDamageSliderTimer = 1f;
                leftPlayerLastHealth = leftPlayerHealth;

                leftPlayerHealthSlider.value = Mathf.Lerp(leftPlayerHealthSlider.value, leftPlayerHealth, healthSliderLerpSpeed * Time.deltaTime);
                if(pauseLeftDamageSliderTimer == 0f)
                    leftPlayerDamageSlider.value = Mathf.Lerp(leftPlayerDamageSlider.value, leftPlayerHealth, healthSliderLerpSpeed * Time.deltaTime);
            }

            // Show right player health/damage.
            if (rightPlayer != null)
            {
                rightPlayerHealth = rightPlayer.GetHealth().CurrentHealth;
                if (rightPlayerHealth != rightPlayerLastHealth) pauseRightDamageSliderTimer = 1f;
                rightPlayerLastHealth = rightPlayerHealth;

                rightPlayerHealthSlider.value = Mathf.Lerp(rightPlayerHealthSlider.value, rightPlayerHealth, healthSliderLerpSpeed * Time.deltaTime);
                if(pauseRightDamageSliderTimer == 0f)
                    rightPlayerDamageSlider.value = Mathf.Lerp(rightPlayerDamageSlider.value, rightPlayerHealth, healthSliderLerpSpeed * Time.deltaTime);
            }
        }

        public void Initiate()
        {
            leftPlayer = SessionManager.instance.GetParticipate(0).GetPlayer();
            rightPlayer = SessionManager.instance.GetParticipate(1).GetPlayer();

            leftPlayerHealthSlider.maxValue = leftPlayer.GetHealth().GetMaxHealth();
            leftPlayerDamageSlider.maxValue = leftPlayer.GetHealth().GetMaxHealth();
            rightPlayerHealthSlider.maxValue = rightPlayer.GetHealth().GetMaxHealth();
            rightPlayerDamageSlider.maxValue = rightPlayer.GetHealth().GetMaxHealth();

            leftPlayerNameText.text = SessionManager.instance.GetParticipate(0).GetPlayer().GetLoadedCharacter().name.ToUpper();
            rightPlayerNameText.text = SessionManager.instance.GetParticipate(1).GetPlayer().GetLoadedCharacter().name.ToUpper();
        }

        // Display the current amount of wins each player has.
        public void DisplayCurrentWins()
        {
            for(int i = 0; i < leftPlayerWinCounters.Length; i++)
            {
                if (SessionManager.instance.GetParticipate(0).Wins > i)
                    leftPlayerWinCounters[i].sprite = winCounterFill;
                else
                    leftPlayerWinCounters[i].sprite = winCounterEmpty;
            }

            for (int i = 0; i < rightPlayerWinCounters.Length; i++)
            {
                if (SessionManager.instance.GetParticipate(1).Wins > i)
                    rightPlayerWinCounters[i].sprite = winCounterFill;
                else
                    rightPlayerWinCounters[i].sprite = winCounterEmpty;
            }
        }

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