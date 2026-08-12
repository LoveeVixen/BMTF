// LOVEEVIXEN
using InputSystem;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Character
{
    public string name = "Character display name";
    public ComboGraph comboGraph;
    public RuntimeAnimatorController runtimeAnimator;

    public bool unlockedByDefault = true;
    public static List<string> isLocked = new List<string>();
    public static int[] highlightColumnIndex = { 1, 2, 1, 1 };
    public static int[] highlightRowIndex = { 1, 1, 5, 6 };
    public static string[] selectedCharacter = 
    {
        "Shirobon",
        "Kurobon",
        "Shirobon",
        "Kurobon",
    };

    public static int[] selectedOutfitIndex = { 0, 0, 1, 1 };
    public static bool[] confirmedSelect = { false, false, false, false };

    [System.Serializable]
    public class Outfit
    {
        public string characterPrefab = "Character prefab path";
        public Color uiColor = Color.white;
    }
    public Outfit[] outfits = new Outfit[2];
    public Sprite[] uiIcon = new Sprite[2];
    public bool showCharacterInMenu = true;

    public static string characterPrefabsPath = "Characters";
    public static float timeBetweenIconAnimationFrames = 0.5f;

    [Header("Faces")]
    public Material normalFace;
    public Material happyFace;
    public Material angryFace;
    public Material hurtFace;
    public Material dizzyFace;

    [Header("Sounds")]
    public string announceNameSound = "Character Name";
    public string attackSound = "Attack";
    public string specialSound = "Special";
    public string hitSound = "Hit";
    public string knockoutSound = "KO";

    public string GetCharacterPath() { return characterPrefabsPath + "/" + outfits[0].characterPrefab; }
    public string GetCharacterPath(int outfitIndex) { return characterPrefabsPath + "/" + outfits[outfitIndex].characterPrefab; }

    // Check if character is not unlocked.
    public bool IsLocked()
    {
        foreach(string lockedCharacterName in isLocked)
        {
            if (name == lockedCharacterName)
                return true;
        }

        return false;
    }

    public static void UndoConfirmedSelectedCharacters()
    {
        for (int i = 0; i < confirmedSelect.Length; i++)
            confirmedSelect[i] = false;
    }
}
