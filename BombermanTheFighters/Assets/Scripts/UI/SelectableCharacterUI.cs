// LOVEEVIXEN
using InputSystem;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SelectableCharacterUI : MonoBehaviour
    {
        [SerializeField] Character character;
        [SerializeField] Image iconDisplay;
        [SerializeField] Text playerHighlightText;
        [SerializeField] Animator anim;
        private Sprite[] iconFrames = new Sprite[2];
        private int columnIndex = 1;
        private int rowIndex = 1;
        private bool isRandomSelect = false;
        private float returnToInitialFrameTime = 0.2f;
        private float returnToInitialFrameTimer = 0f;
        private CharacterSelectUI characterSelectMenu;

        private void Update()
        {
            if(returnToInitialFrameTimer > 0f)
            {
                returnToInitialFrameTimer -= Time.deltaTime;
                if (returnToInitialFrameTimer <= 0f)
                {
                    returnToInitialFrameTimer = 0f;
                    iconDisplay.sprite = iconFrames[0];
                }
            }
        }

        public void SetCharacter(Character setCharacter)
        {
            isRandomSelect = false;
            character = setCharacter;
            iconFrames = setCharacter.uiIcon;
            iconDisplay.sprite = iconFrames[0];
        }

        public void SetToRandomSelect()
        {
            isRandomSelect = true;
            character = null;

            CharacterSelectUI characterSelectUI = FindFirstObjectByType<CharacterSelectUI>();
            iconDisplay.sprite = characterSelectUI.GetRandomSelectIcon();
        }

        public void SetColumnIndex(int set) { columnIndex = set; }
        public void SetRowIndex(int set) { rowIndex = set; }

        public static SelectableCharacterUI Find(int column, int row)
        {
            CharacterSelectUI characterSelectUI = FindFirstObjectByType<CharacterSelectUI>();
            SelectableCharacterUI[] icons = characterSelectUI.GetSelectableCharacterUI().ToArray();
            foreach(SelectableCharacterUI icon in icons)
            {
                if (icon.columnIndex == column && icon.rowIndex == row)
                    return icon;
            }

            return null;
        }

        public void UpdateHighlight(bool[] playersHighlighting)
        {
            int playersHighlightingAmount = 0;
            string displayText = "";

            for (int i = 0; i < playersHighlighting.Length; i++)
            {
                if (playersHighlighting[i])
                    playersHighlightingAmount++;
            }

            if (playersHighlightingAmount == 0)
                anim.Play("Normal");
            else
            {
                anim.Play("Highlight");
                for (int i = 0; i < playersHighlighting.Length; i++)
                {
                    if (playersHighlighting[i])
                        displayText += "P" + (i + 1) + " ";
                }
            }

            playerHighlightText.text = displayText;
        }

        public void PlaySelectAnimation()
        {
            if (!isRandomSelect)
            {
                returnToInitialFrameTimer = returnToInitialFrameTime;
                iconDisplay.sprite = iconFrames[1];
            }
        }

        public Character GetCharacter() { return character; }
        public bool IsRandomSelect() { return isRandomSelect; }
        public void SetCharacterSelectMenu(CharacterSelectUI set) {  characterSelectMenu = set; }
    }
}