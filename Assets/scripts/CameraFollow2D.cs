using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;     // Sarah
    public float smoothSpeed = 5f;
    public Vector3 offset;       // optional

    void LateUpdate()
    {
        if (target == null) return;

        // Follow only on X, keep current Y and Z
        Vector3 desiredPosition = new Vector3(
            target.position.x,
            transform.position.y,
            transform.position.z
        ) + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}
