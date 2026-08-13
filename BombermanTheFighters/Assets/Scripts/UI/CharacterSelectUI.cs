// LOVEEVIXEN
using Audio;
using InputSystem;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CharacterSelectUI : MonoBehaviour
    {
        [SerializeField] bool openMenuOnStart = true;
        [SerializeField] bool showCharacterDisplay = true;
        [SerializeField] bool playMusicOnOpen = true;
        [SerializeField] bool resetConfirmSelectOnOpen = false;
        [SerializeField] string playMusic = "";
        [SerializeField] Transform selectableCharacterIconContainer;
        [SerializeField] SelectableCharacterUI selectableCharacterUIPrefab;
        [SerializeField] CharacterDisplay characterDisplayPrefab;
        private CharacterDisplay[] characterDisplays = new CharacterDisplay[2];
        [SerializeField] Text timerText;
        [SerializeField] Sprite randomSelectIcon;
        private enum DoAfterCharacterSelect { gameScene, returnToOnlineLobby };
        [SerializeField] DoAfterCharacterSelect doAfterCharacterSelect = DoAfterCharacterSelect.gameScene;
        [SerializeField] float exitAfterConfirmDelayTime = 2f;
        private float exitAfterConfirmDelayTimer;
        private SelectableCharacterUI[] highlightedCharacters = new SelectableCharacterUI[4];
        private List<SelectableCharacterUI> selectableCharacterUI = new List<SelectableCharacterUI>();
        [SerializeField] bool timeLimit = true;
        [SerializeField] float timer = 20f;
        [SerializeField] GridLayoutGroup grid;
        [SerializeField] Menu menu;
        private Camera cam;
        private bool initiated;
        private bool opened;
        private bool closed = true;

        private string playSoundOnHighlight = "Menu_Highlight";
        private string playSoundOnSelect = "Menu_Select";
        private string playSoundOnError = "Menu_Error";

        private int amountOfColumns = 0;
        private int amountOfRows = 1;

        private void Awake()
        {
            menu = GetComponent<Menu>();
            exitAfterConfirmDelayTimer = exitAfterConfirmDelayTime;
            cam = Camera.main;
        }

        private void Start()
        {
            if (openMenuOnStart)
                menu.Open();
        }

        // Update is called once per frame
        void Update()
        {
            if (menu.IsOpen())
            {
                if (!opened)
                {
                    opened = true;
                    closed = false;
                    OnOpen();
                }

                // Player input.
                for (int i = 0; i < GameManager.instance.IsPlaying().Length; i++)
                {
                    // If the player is playing, allow them to have input functionality.
                    if (GameManager.instance.IsPlaying()[i])
                    {
                        if (!Character.confirmedSelect[i])
                        {
                            if (InputReader.AllPlayersInputData()[i].pressingUp)
                                MoveUp(i);
                            else if (InputReader.AllPlayersInputData()[i].pressingDown)
                                MoveDown(i);

                            if (InputReader.AllPlayersInputData()[i].pressingRight)
                                MoveRight(i);
                            else if (InputReader.AllPlayersInputData()[i].pressingLeft)
                                MoveLeft(i);

                            // Confirm selected character outfit.
                            if (InputReader.AllPlayersInputData()[i].pressing0)
                            {
                                if (!highlightedCharacters[i].IsRandomSelect())
                                    ConfirmSelectedCharacter(i, highlightedCharacters[i].GetCharacter(), 0);
                                else
                                    ConfirmSelectedCharacter(i, RandomAvailableCharacter(), 0);
                            }
                            else if (InputReader.AllPlayersInputData()[i].pressing3)
                            {
                                if (!highlightedCharacters[i].IsRandomSelect())
                                    ConfirmSelectedCharacter(i, highlightedCharacters[i].GetCharacter(), 1);
                                else
                                    ConfirmSelectedCharacter(i, RandomAvailableCharacter(), 1);
                            }
                        }
                    }
                }

                // Tick down exit time after all players have confirmed their characters.
                if (AllPlayersConfirmedCharacters() && exitAfterConfirmDelayTimer > 0f)
                {
                    exitAfterConfirmDelayTimer -= Time.deltaTime;
                    if (exitAfterConfirmDelayTimer < 0f)
                        exitAfterConfirmDelayTimer = 0f;
                }
                else if (!AllPlayersConfirmedCharacters() && exitAfterConfirmDelayTimer != exitAfterConfirmDelayTime)
                    exitAfterConfirmDelayTimer = exitAfterConfirmDelayTime;

                // Exit character select after confirming all player's characters.
                if (exitAfterConfirmDelayTimer == 0f)
                {
                    selectableCharacterUI.RemoveRange(0, selectableCharacterUI.Count);
                    menu.Close();
                    if (doAfterCharacterSelect == DoAfterCharacterSelect.gameScene)
                        PhotonNetwork.LoadLevel("Game");
                    else if(doAfterCharacterSelect == DoAfterCharacterSelect.returnToOnlineLobby)
                    {
                        NetworkSessionLobby lobby = FindFirstObjectByType<NetworkSessionLobby>();
                        lobby.GetMenu().Open();
                    }
                }

                // Tick down timer until everyone is forced to pick their characters.
                if (timer > 0f && timeLimit)
                {
                    timer -= Time.deltaTime;
                    if (timer < 0f)
                        timer = 0f;
                }

                // Force anyone who hasn't selected a character yet to pick their character once timer has reached zero.
                if (timer == 0f && !AllPlayersConfirmedCharacters() && timeLimit)
                {
                    for (int i = 0; i < Character.confirmedSelect.Length; i++)
                    {
                        if (!Character.confirmedSelect[i])
                            ConfirmSelectedCharacter(i, RandomAvailableCharacter(), 0);
                    }
                }

                // Display time left for everyone to pick a character.
                if (timeLimit)
                    timerText.text = Mathf.RoundToInt(timer).ToString();
                else
                    timerText.text = "";
            }
            else if(!closed)
            {
                closed = true;
                opened = false;
                OnClose();
            }
        }

        void Initiate()
        {
            if (!initiated)
            {
                initiated = true;

                int column = 0;
                int row = 1;
                for (int i = 0; i < GameManager.instance.GetCharactersList().Count + 1; i++)
                {
                    // Move to next row.
                    if (column > grid.constraintCount - 1)
                    {
                        column = 0;
                        row++;
                        amountOfRows = row;
                    }

                    if (i != GameManager.instance.GetCharactersList().Count)
                    {
                        Character character = GameManager.instance.GetCharactersList()[i];
                        if (character.showCharacterInMenu)
                        {
                            // Move to next column.
                            column++;
                            if (column > amountOfColumns)
                                amountOfColumns = column;

                            // Add character to menu.
                            SelectableCharacterUI icon = Instantiate(selectableCharacterUIPrefab, selectableCharacterIconContainer);
                            icon.SetCharacter(character);
                            icon.SetColumnIndex(column);
                            icon.SetRowIndex(row);
                            icon.SetCharacterSelectMenu(this);
                            selectableCharacterUI.Add(icon);
                        }
                    }
                    else
                    {
                        // Setup random select for last index in for loop.
                        column++;
                        if (column > amountOfColumns)
                            amountOfColumns = column;

                        SelectableCharacterUI icon = Instantiate(selectableCharacterUIPrefab, selectableCharacterIconContainer);
                        icon.SetToRandomSelect();
                        icon.SetColumnIndex(column);
                        icon.SetRowIndex(row);
                        icon.SetCharacterSelectMenu(this);
                        selectableCharacterUI.Add(icon);
                    }
                }

                // Set default highlighted icons.
                for (int i = 0; i < highlightedCharacters.Length; i++)
                    highlightedCharacters[i] = SelectableCharacterUI.Find(Character.highlightColumnIndex[i], Character.highlightRowIndex[i]);

                // Show the characters highlighted for select.
                ShowHighlightedCharacters();

                // Setup character displays.
                if (showCharacterDisplay)
                {
                    Vector3 forwardOffset = cam.transform.forward * 10f;
                    Vector3 sideOffset = cam.transform.right * 6f;
                    Vector3 upOffset = cam.transform.up * -6f;

                    characterDisplays[0] = Instantiate(characterDisplayPrefab, forwardOffset + -sideOffset + upOffset, Quaternion.Euler(0f, 90f, 0f));
                    if (GameManager.instance.IsPlaying()[0]) characterDisplays[0].DisplayCharacter(highlightedCharacters[0].GetCharacter().name, Character.selectedOutfitIndex[0]);

                    characterDisplays[1] = Instantiate(characterDisplayPrefab, forwardOffset + sideOffset + upOffset, Quaternion.Euler(0f, 270f, 0f));
                    if (GameManager.instance.IsPlaying()[1]) characterDisplays[1].DisplayCharacter(highlightedCharacters[1].GetCharacter().name, Character.selectedOutfitIndex[1]);
                }

                // Play character select music.
                if (playMusicOnOpen) AudioManager.instance.PlayMusic(playMusic);
            }
        }

        void OnOpen()
        {
            if(!initiated) Initiate();
            ShowHighlightedCharacters();
            if(resetConfirmSelectOnOpen)
                Character.UndoConfirmedSelectedCharacters();
        }

        void OnClose()
        {
            
        }

        // Local player input.
        #region
        void ConfirmSelectedCharacter(int localPlayer, Character character, int outfitIndex)
        {
            SelectableCharacterUI icon = SelectableCharacterUI.Find(Character.highlightColumnIndex[localPlayer], Character.highlightRowIndex[localPlayer]);
            bool outfitIsTaken = IsCharacterOutfitTaken(character, outfitIndex);
            bool characterIsLocked = character.IsLocked();
            int useOutfit = outfitIndex;
            bool confirmedSelectedCharacter = false;

            // Play select animation.
            if (icon != null)
                icon.PlaySelectAnimation();

            if (!outfitIsTaken && !characterIsLocked)
                confirmedSelectedCharacter = true; // character has been selected.
            else if(characterIsLocked)
                AudioManager.instance.PlayNonDiegeticSound(playSoundOnError); // Deny select, character is locked.
            else if(outfitIsTaken)
            {
                // Force player to pick different outfit for selected character if it's already taken.
                for(int i = 0; i < character.outfits.Length; i++)
                {
                    if (!IsCharacterOutfitTaken(character, i))
                    {
                        useOutfit = i;
                        confirmedSelectedCharacter = true;
                        break;
                    }
                }
            }

            // Confirm selected character.
            if(confirmedSelectedCharacter)
            {
                Character.confirmedSelect[localPlayer] = true;
                Character.selectedCharacter[localPlayer] = character.name;
                Character.selectedOutfitIndex[localPlayer] = useOutfit;

                ShowHighlightedCharacters();
                AudioManager.instance.PlayNonDiegeticSound(playSoundOnSelect);

                // Display confirmed character.
                if(showCharacterDisplay) characterDisplays[localPlayer].DisplayCharacter(Character.selectedCharacter[localPlayer], useOutfit);
            }
        }

        void CancelSelectedCharacter(int localPlayer)
        {
            Character.confirmedSelect[localPlayer] = false;
            ShowHighlightedCharacters();
        }

        void MoveUp(int localPlayer)
        {
            Character.highlightRowIndex[localPlayer]++;
            if (Character.highlightRowIndex[localPlayer] > amountOfRows)
                Character.highlightRowIndex[localPlayer] = 1;

            highlightedCharacters[localPlayer] = SelectableCharacterUI.Find(Character.highlightColumnIndex[localPlayer], Character.highlightRowIndex[localPlayer]);
            if (highlightedCharacters[localPlayer] == null)
                MoveUp(localPlayer);
            else
            {
                AudioManager.instance.PlayNonDiegeticSound(playSoundOnHighlight);

                // Display newly highlighted character
                if (!highlightedCharacters[localPlayer].IsRandomSelect() && showCharacterDisplay)
                    characterDisplays[localPlayer].DisplayCharacter(highlightedCharacters[localPlayer].GetCharacter().name, 0);
                else if (showCharacterDisplay)
                    characterDisplays[localPlayer].DisplayCharacter("", 0);
            }

            ShowHighlightedCharacters();
        }

        void MoveRight(int localPlayer)
        {
            Character.highlightColumnIndex[localPlayer]++;
            if (Character.highlightColumnIndex[localPlayer] > amountOfColumns)
                Character.highlightColumnIndex[localPlayer] = 1;

            highlightedCharacters[localPlayer] = SelectableCharacterUI.Find(Character.highlightColumnIndex[localPlayer], Character.highlightRowIndex[localPlayer]);
            if (highlightedCharacters[localPlayer] == null)
                MoveRight(localPlayer);
            else
            {
                AudioManager.instance.PlayNonDiegeticSound(playSoundOnHighlight);

                // Display newly highlighted character
                if (!highlightedCharacters[localPlayer].IsRandomSelect() && showCharacterDisplay)
                    characterDisplays[localPlayer].DisplayCharacter(highlightedCharacters[localPlayer].GetCharacter().name, 0);
                else if (showCharacterDisplay)
                    characterDisplays[localPlayer].DisplayCharacter("", 0);
            }

            ShowHighlightedCharacters();
        }

        void MoveDown(int localPlayer)
        {
            Character.highlightRowIndex[localPlayer]--;
            if (Character.highlightRowIndex[localPlayer] < 1)
                Character.highlightRowIndex[localPlayer] = amountOfRows;

            highlightedCharacters[localPlayer] = SelectableCharacterUI.Find(Character.highlightColumnIndex[localPlayer], Character.highlightRowIndex[localPlayer]);
            if (highlightedCharacters[localPlayer] == null)
                MoveDown(localPlayer);
            else
            {
                AudioManager.instance.PlayNonDiegeticSound(playSoundOnHighlight);

                // Display newly highlighted character
                if (!highlightedCharacters[localPlayer].IsRandomSelect() && showCharacterDisplay)
                    characterDisplays[localPlayer].DisplayCharacter(highlightedCharacters[localPlayer].GetCharacter().name, 0);
                else if (showCharacterDisplay)
                    characterDisplays[localPlayer].DisplayCharacter("", 0);
            }

            ShowHighlightedCharacters();
        }

        void MoveLeft(int localPlayer)
        {
            Character.highlightColumnIndex[localPlayer]--;
            if (Character.highlightColumnIndex[localPlayer] < 1)
                Character.highlightColumnIndex[localPlayer] = amountOfColumns;

            highlightedCharacters[localPlayer] = SelectableCharacterUI.Find(Character.highlightColumnIndex[localPlayer], Character.highlightRowIndex[localPlayer]);
            if (highlightedCharacters[localPlayer] == null)
                MoveLeft(localPlayer);
            else
            {
                AudioManager.instance.PlayNonDiegeticSound(playSoundOnHighlight);

                // Display newly highlighted character
                if (!highlightedCharacters[localPlayer].IsRandomSelect() && showCharacterDisplay)
                    characterDisplays[localPlayer].DisplayCharacter(highlightedCharacters[localPlayer].GetCharacter().name, 0);
                else if(showCharacterDisplay)
                    characterDisplays[localPlayer].DisplayCharacter("", 0);
            }

            ShowHighlightedCharacters();
        }
        #endregion

        public void ShowHighlightedCharacters()
        {
            foreach(SelectableCharacterUI icon in selectableCharacterUI)
            {
                bool[] playersHighlighting = new bool[4];

                for(int i = 0; i < GameManager.instance.IsPlaying().Length; i++)
                {
                    if (icon == highlightedCharacters[i] && !Character.confirmedSelect[i] && GameManager.instance.IsPlaying()[i])
                        playersHighlighting[i] = true;
                }

                icon.UpdateHighlight(playersHighlighting);
            }
        }

        bool AllPlayersConfirmedCharacters()
        {
            bool allPlayersConfirmed = true;
            for (int i = 0; i < GameManager.instance.IsPlaying().Length; i++)
            {
                if (!Character.confirmedSelect[i] && GameManager.instance.IsPlaying()[i])
                {
                    allPlayersConfirmed = false;
                    break;
                }
            }

            return allPlayersConfirmed;
        }

        Character RandomAvailableCharacter()
        {
            List<Character> availableCharacters = new List<Character>();

            foreach (Character character in GameManager.instance.GetCharactersList())
            {
                // Check that the character is selectable in menu and isn't locked.
                if (character.showCharacterInMenu && !character.IsLocked())
                    availableCharacters.Add(character);
            }

            int randomIndex = Random.Range(0, availableCharacters.Count);
            return availableCharacters[randomIndex];
        }

        bool IsCharacterOutfitTaken(Character character, int outfitIndex)
        {
            for(int i = 0; i < Character.selectedCharacter.Length; i++)
            {
                if (Character.confirmedSelect[i])
                {
                    if (Character.selectedCharacter[i] == character.name && Character.selectedOutfitIndex[i] == outfitIndex)
                        return true;
                }
            }

            return false;
        }

        public Menu GetMenu() { return menu; }
        public Sprite GetRandomSelectIcon() { return randomSelectIcon; }
        public bool ShowCharacterDisplay() {  return showCharacterDisplay; }
        public CharacterDisplay[] GetCharacterDisplays() { return characterDisplays; }
        public SelectableCharacterUI[] GetHighlightedCharacters() { return highlightedCharacters; }
        public List<SelectableCharacterUI> GetSelectableCharacterUI() { return selectableCharacterUI; }
    }
}