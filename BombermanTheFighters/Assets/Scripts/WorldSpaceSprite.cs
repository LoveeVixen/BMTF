// LOVEEVIXEN
using UnityEngine;

public class WorldSpaceSprite : MonoBehaviour
{
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    void FixedUpdate()
    {
        // Always look at camera.
        transform.LookAt(cam.transform);
    }
}
