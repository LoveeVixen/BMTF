// LOVEEVIXEN
using UnityEngine;

public class RotationAnimation : MonoBehaviour
{
    [SerializeField] Vector3 rotateDirection;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotateDirection * Time.deltaTime);
    }
}
