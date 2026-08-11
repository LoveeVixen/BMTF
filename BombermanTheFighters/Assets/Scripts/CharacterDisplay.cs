// LOVEEVIXEN
using EntitySystem;
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class CharacterDisplay : MonoBehaviour
{
    private GameObject loadedCharacterPrefab;
    private Character loadedCharacter;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void DisplayCharacter(string characterName, int outfitIndex)
    {
        StartCoroutine(IDisplayCharacter(characterName, outfitIndex));
    }

    IEnumerator IDisplayCharacter(string characterName, int outfitIndex)
    {
        // Clear last loaded character prefab if there is one.
        if (loadedCharacterPrefab != null)
            Destroy(loadedCharacterPrefab);

        if (characterName != "")
        {
            Character characterData = GameManager.instance.FindCharacter(characterName);

            // Instantiate new character prefab into display gameobject.
            GameObject characterObj = (GameObject)Instantiate(Resources.Load(characterData.GetCharacterPath(outfitIndex)), transform.position, Quaternion.identity, transform);
            characterObj.transform.Rotate(transform.rotation.eulerAngles);
            characterObj.name = "Character";
            loadedCharacterPrefab = characterObj;
            loadedCharacter = characterData;
            gameObject.name = characterName;
            loadedCharacterPrefab.SetActive(false);

            // Reset animator so playing idle animation later doesn't bug.
            anim.runtimeAnimatorController = null;
            anim.Play("Run");

            yield return new WaitForSeconds(0.02f);

            // Setup animator.
            loadedCharacterPrefab.SetActive(true);
            anim.runtimeAnimatorController = characterData.runtimeAnimator;
            anim.Play("Idle");
        }
    }
}
