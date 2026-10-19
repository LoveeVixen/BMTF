// LOVEEVIXEN
using UnityEngine;

public class WeatherParticles : MonoBehaviour
{
    private Camera cam;
    private Vector3 offset = new Vector3(0f, 16f, 0f);

    private void Awake()
    {
        cam = FindFirstObjectByType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = cam.transform.position + offset;
    }
}
