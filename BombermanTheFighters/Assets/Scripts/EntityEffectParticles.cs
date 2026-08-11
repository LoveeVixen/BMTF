// LOVEEVIXEN
using UnityEngine;

public class EntityEffectParticles : MonoBehaviour
{
    private Transform followTarget;

    // Update is called once per frame
    void Update()
    {
        transform.position = followTarget.position;
    }

    public void SetFollowTarget(Transform set) { followTarget = set; }

}
