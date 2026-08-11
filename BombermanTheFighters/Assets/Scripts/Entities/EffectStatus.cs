// LOVEEVIXEN
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EffectStatus
{
    public string name;
    public string reverseName;
    public ParticleSystem particlesPrefab;
}

[System.Serializable]
public class AppliedEffectStatus
{
    public int effectIndex;
    public int multiply = 1;
    public float lastTime = 5f;
    public List<ParticleSystem> effectParticles = new List<ParticleSystem>();

    public AppliedEffectStatus(int effectIndex, int multiply, float lastTime)
    {
        this.effectIndex = effectIndex;
        this.multiply = multiply;
        this.lastTime = lastTime;
    }
}