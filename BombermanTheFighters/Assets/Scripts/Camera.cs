// LOVEEVIXEN
using UnityEngine;

public class Camera : MonoBehaviour
{
    private Transform target;
    [SerializeField] Vector3 lookAtOffset = new Vector3(0f, 6f, 0f);
    [SerializeField][Range(10f, 30f)] float forwardOffset = 16f;
    [SerializeField][Range(2f, 10f)] float upOffset = 6f;
    [SerializeField][Range(1f, 50f)] float lerpSpeed = 5f;

    private void Awake()
    {
        target = GameObject.Find("PlayerCenterPosition").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookAt = new Vector3(target.position.x + lookAtOffset.x, target.position.y + lookAtOffset.y, target.position.z + lookAtOffset.z);
        transform.LookAt(lookAt);

        Vector3 currentPos = transform.position;
        Vector3 targetPos = target.position + (target.forward * -forwardOffset) + (target.up * upOffset);
        transform.position = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * lerpSpeed);
    }
}