using UnityEngine;

public class SpectatorMovement : MonoBehaviour
{

	public float moveSpeedSlow = 5f;
	public float moveSpeedNormal = 10f;
	public float moveSpeedFast = 40f;
	public float maxDistance = 300f;

    private enum SpectatorMoveState { Slow, Normal, Fast }
	SpectatorMoveState moveState = SpectatorMoveState.Normal;

	private void Update()
	{
		float moveSpeed = moveSpeedNormal;

        if (Input.GetKey(KeyCode.LeftShift))
            moveState = SpectatorMoveState.Fast;
        else if (Input.GetKey(KeyCode.LeftControl))
            moveState = SpectatorMoveState.Slow;
        else
            moveState = SpectatorMoveState.Normal;

        switch (moveState)
		{
			case SpectatorMoveState.Slow:
				moveSpeed = moveSpeedSlow;
				break;
			case SpectatorMoveState.Normal:
				moveSpeed = moveSpeedNormal;
				break;
			case SpectatorMoveState.Fast:
				moveSpeed = moveSpeedFast;
				break;
		}

		transform.position += transform.forward * moveSpeed * Time.deltaTime * Input.GetAxis("Vertical");
		transform.position += transform.right * moveSpeed * Time.deltaTime * Input.GetAxis("Horizontal");

		if (Vector3.Distance(Vector3.zero, transform.position) > maxDistance)
			transform.position = Vector3.zero;
	}
}