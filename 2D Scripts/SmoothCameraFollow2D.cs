using UnityEngine;

public class SmoothCameraFollow2D : MonoBehaviour
{
    public Transform target;
    public enum CameraFollowUpdateMode { Update, FixedUpdate, LateUpdate }
    public CameraFollowUpdateMode updateMode = CameraFollowUpdateMode.FixedUpdate;
    public float smoothSpeed = 0.1f;
    public float zPosition = -100;
    public Vector2 offset;

    [Header("Size with Velocity (optional)")]
    public bool sizeWithVelocity = false;
    public float defaultSize = 5.0f;
    public float velocityMultiplier = 1.0f;
    public new Camera camera;
    public new Rigidbody2D rigidbody;

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

            if (sizeWithVelocity)
            {
                float velocity = rigidbody.velocity.magnitude;
                camera.orthographicSize = defaultSize + velocity * velocityMultiplier;
            }
        }
    }
}
