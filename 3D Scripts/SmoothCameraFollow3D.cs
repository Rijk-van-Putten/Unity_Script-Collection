using UnityEngine;

public class SmoothCameraFollow3D : MonoBehaviour
{
    public Transform target;
    public enum CameraFollowUpdateMode { Update, FixedUpdate, LateUpdate }
    public CameraFollowUpdateMode updateMode = CameraFollowUpdateMode.FixedUpdate;
    public float smoothSpeed = 0.1f;
    public Vector3 offset;

    private void Start()
    {
       
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
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
