using UnityEngine;

public class FPCameraController : MonoBehaviour
{
	[Header("Default Settings")]
	public float rotationRange = 160f; 
	public float rotationSpeed = 2;
	public float dampingTime = 0.2f;
	public bool relative = true;
    public bool zooming;

	[Header("Character Rotating")]
	public Transform characterTarget;
	public Vector3 axisMultipliers = Vector3.forward;

    [Header("Player Settings")]
    public bool enablePlayerSettings = true;
	public float sensitivity = 100.0f;
	public float zoomSensitivity = 0.1f;

    private Vector3 targetAngles;
    private Vector3 followAngles;
    private Vector3 followVelocity;
    private Quaternion originalRotation;

	void Start()
    {
        if (enablePlayerSettings)
        {
            sensitivity = PlayerSettings.instance.GetFloat("mouse_sensitivity");
            zoomSensitivity = PlayerSettings.instance.GetFloat("zoom_sensitivity");
        }

		originalRotation = transform.localRotation;
        rotationSpeed = sensitivity / 200;
	}

	void Update()
    {
		if (!CursorController.IsInGame)
			return;
		transform.localRotation = originalRotation;

        if (zooming)
            rotationSpeed = sensitivity / 50 * zoomSensitivity;
        else
            rotationSpeed = sensitivity / 50;

        float inputH = 0;
		float inputV = 0;
		if (relative)
		{
			inputH = Input.GetAxis("Mouse X");
			inputV = Input.GetAxis("Mouse Y");
			
			if (targetAngles.y > 180) { targetAngles.y -= 360; followAngles.y -= 360; }
			if (targetAngles.x > 180) { targetAngles.x -= 360; followAngles.x-= 360; }
			if (targetAngles.y < -180) { targetAngles.y += 360; followAngles.y += 360; }
			if (targetAngles.x < -180) { targetAngles.x += 360; followAngles.x += 360; }

			targetAngles.y += inputH * rotationSpeed;
			targetAngles.x += inputV * rotationSpeed;

			targetAngles.x = Mathf.Clamp ( targetAngles.x, -rotationRange * 0.5f, rotationRange * 0.5f );
		}
		else
        {
			inputH = Input.mousePosition.x;
			inputV = Input.mousePosition.y;

			targetAngles.y = Mathf.Lerp ( -rotationRange * 0.5f, rotationRange * 0.5f, inputH/Screen.width );
			targetAngles.x = Mathf.Lerp ( -rotationRange * 0.5f, rotationRange * 0.5f, inputV/Screen.height );
		}

		followAngles = Vector3.SmoothDamp( followAngles, targetAngles, ref followVelocity, dampingTime );
		
		transform.localRotation = originalRotation * Quaternion.Euler( -followAngles.x, followAngles.y, 0);
		if (characterTarget)
			characterTarget.localRotation = Quaternion.Euler(transform.localEulerAngles.x * axisMultipliers.x, transform.localEulerAngles.y * axisMultipliers.y, transform.localEulerAngles.z * axisMultipliers.z);
	}
}