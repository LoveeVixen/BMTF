// LOVEEVIXEN
using UnityEngine;

public class Entity : MonoBehaviour
{
    // Round entity's position to be by 1 decimal place.
    public virtual void SnapPosition()
    {
        float x = Mathf.Round(transform.position.x * 10f) * 0.1f;
        float y = Mathf.Round(transform.position.y * 10f) * 0.1f;
        float z = Mathf.Round(transform.position.z * 10f) * 0.1f;
        transform.position = new Vector3(x, y, z);
    }

    public Vector3 Pos()
    {
        return transform.position;
    }
}
