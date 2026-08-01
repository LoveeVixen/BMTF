// LOVEEVIXEN
using InputSystem;
using UnityEngine;

[System.Serializable]
public class Character
{
    public string name = "Character display name";
    public ComboGraph comboGraph;
    public RuntimeAnimatorController runtimeAnimator;

    [System.Serializable]
    public class Outfit
    {
        public string characterPrefab = "Character prefab path";
        public Color uiColor = Color.white;
    }
    public Outfit[] outfits = new Outfit[2];
    public Sprite[] uiIcon;
    public bool showCharacterInMenu = true;

    public static string characterPrefabsPath = "Characters";
    public static float timeBetweenIconAnimationFrames = 0.5f;

    [Header("Sounds")]
    public string announceNameSound = "Character Name";
    public string attackSound = "Attack";
    public string specialSound = "Special";
    public string hitSound = "Hit";
    public string knockoutSound = "KO";

    public string GetCharacterPath() { return characterPrefabsPath + "/" + outfits[0].characterPrefab; }
    public string GetCharacterPath(int outfitIndex) { return characterPrefabsPath + "/" + outfits[outfitIndex].characterPrefab; }
}
