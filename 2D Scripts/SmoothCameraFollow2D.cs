using UnityEngine;

public class SmoothCameraFollow2D : MonoBehaviour
{
    public Transform target;
    public enum CameraFollowUpdateMode { Update, FixedUpdate, LateUpdate }
    public CameraFollowUpdateMode updateMode = CameraFollowUpdateMode.FixedUpdate;
    public float smoothSpeed = 0.1f;
    public float zPosition = -100;
    public Vector2 offset;

    private void Start()
    {
        transform.position = target.position;
    }

    void FixedUpdate()
    {
        if (updateMode == CameraFollowUpdateMode.FixedUpdate)
            Follow();
    }

    private void Update()
    {
        if (updateMode == CameraFollowUpdateMode.Update)
            Follow();
    }

    private void LateUpdate()
    {
        if (updateMode == CameraFollowUpdateMode.LateUpdate)
            Follow();
    }

    void Follow()
    {
        if (target)
        {
            Vector2 desiredPosition = new Vector2(target.position.x, target.position.y) + offset;
            Vector2 smoothedPosition = Vector2.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, zPosition);
        }
    }
}
