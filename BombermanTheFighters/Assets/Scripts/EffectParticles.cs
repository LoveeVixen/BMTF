// LOVEEVIXEN
using UnityEngine;

public class EffectParticles : MonoBehaviour
{
    private ParticleSystem particleSystem;

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        particleSystem.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!particleSystem.isPlaying)
            Destroy(gameObject);
    }
}
