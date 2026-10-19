// LOVEEVIXEN
using Audio;
using System.Collections;
using UnityEngine;

public class StormSystem : MonoBehaviour
{
    [SerializeField] GameObject stormLight;
    private float strikeThunderMinTime = 3f;
    private float strikeThunderMaxTime = 12f;
    private float strikeThunderTimer;
    private int flickerAmountMin = 2;
    private int flickerAmountMax = 8;
    private int flickerAmount;

    private void Awake()
    {
        ResetTimer();
    }

    private void Update()
    {
        if(strikeThunderTimer > 0f)
        {
            strikeThunderTimer -= Time.deltaTime;
            if(strikeThunderTimer <= 0f)
                StrikeLightning();
        }
    }

    void StrikeLightning()
    {
        StartCoroutine(IStrikeLightning());
    }

    IEnumerator IStrikeLightning()
    {
        SoundProperties props = new SoundProperties();
        props.pitch = Random.Range(0.85f, 1.15f);
        AudioManager.instance.PlayNonDiegeticSound("Thunder", props);

        for(int i = 0; i < flickerAmount; i++)
        {
            stormLight.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            stormLight.SetActive(false);
            yield return new WaitForSeconds(0.04f);
        }

        ResetTimer();
    }

    void ResetTimer()
    {
        strikeThunderTimer = Random.Range(strikeThunderMinTime, strikeThunderMaxTime);
        flickerAmount = Random.Range(flickerAmountMin, flickerAmountMax);
    }
}
