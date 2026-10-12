// LOVEEVIXEN
using EntitySystem;
using Photon.Pun;
using UnityEngine;
using static Frame;

public class ReplayDummy : MonoBehaviour
{
    private GameObject loadedCharacterPrefab;
    private Character loadedCharacter;
    private Face face;
    private Animator anim;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.playbackTime = 0f;
    }

    public void Show()
    {
        if (loadedCharacterPrefab != null) loadedCharacterPrefab.SetActive(true);
    }

    public void Hide()
    {
        if(loadedCharacterPrefab != null) loadedCharacterPrefab.SetActive(false);
    }

    public void LoadCharacter(string characterName, int outfitIndex)
    {
        Character characterData = GameManager.instance.FindCharacter(characterName);

        // Clear last loaded character prefab if there is one.
        if (loadedCharacterPrefab != null)
            Destroy(loadedCharacterPrefab);

        // Instantiate new character prefab into player.
        GameObject characterObj = (GameObject)Instantiate(Resources.Load(characterData.GetCharacterPath(outfitIndex)), transform.position, Quaternion.identity, transform);
        characterObj.transform.Rotate(transform.rotation.eulerAngles);
        characterObj.name = "Character";
        loadedCharacterPrefab = characterObj;
        face = characterObj.GetComponentInChildren<Face>();
        if(face != null) face.SetCharacter(characterData);
        loadedCharacter = characterData;
        anim.runtimeAnimatorController = characterData.runtimeAnimator;
        anim.Play("Idle");
    }

    public Animator GetAnim() { return anim; }
}