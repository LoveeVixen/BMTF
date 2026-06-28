// LOVEEVIXEN
using UnityEngine;

public class Cam : MonoBehaviour
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
        // Make camera always face center of both players.
        Vector3 lookAt = new Vector3(target.position.x + lookAtOffset.x, target.position.y + lookAtOffset.y, target.position.z + lookAtOffset.z);
        transform.LookAt(lookAt);

        // Move camera position to it's target destination.
        Vector3 currentPos = transform.position;
        Vector3 targetPos = target.position + (target.forward * -forwardOffset) + (target.up * upOffset);
        transform.position = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * lerpSpeed);

        // Update stage planes if camera moves too far away from center plane.
        float movePlaneDistance = 115f;
        if (Vector3.Distance(currentPos, Stage.GetNorthWestPlane().transform.position) < movePlaneDistance)
            Stage.MovePlaneTo(Stage.GetNorthWestPlane().transform.position);

        if (Vector3.Distance(currentPos, Stage.GetNorthPlane().transform.position) < movePlaneDistance)
            Stage.MovePlaneTo(Stage.GetNorthPlane().transform.position);

        if (Vector3.Distance(currentPos, Stage.GetNorthEastPlane().transform.position) < movePlaneDistance)
            Stage.MovePlaneTo(Stage.GetNorthEastPlane().transform.position);

        if (Vector3.Distance(currentPos, Stage.GetEastPlane().transform.position) < movePlaneDistance)
            Stage.MovePlaneTo(Stage.GetEastPlane().transform.position);

        if (Vector3.Distance(currentPos, Stage.GetSouthEastPlane().transform.position) < movePlaneDistance)
            Stage.MovePlaneTo(Stage.GetSouthEastPlane().transform.position);

        if (Vector3.Distance(currentPos, Stage.GetSouthPlane().transform.position) < movePlaneDistance)
            Stage.MovePlaneTo(Stage.GetSouthPlane().transform.position);

        if (Vector3.Distance(currentPos, Stage.GetSouthWestPlane().transform.position) < movePlaneDistance)
            Stage.MovePlaneTo(Stage.GetSouthWestPlane().transform.position);

        if (Vector3.Distance(currentPos, Stage.GetWestPlane().transform.position) < movePlaneDistance)
            Stage.MovePlaneTo(Stage.GetWestPlane().transform.position);
    }
}