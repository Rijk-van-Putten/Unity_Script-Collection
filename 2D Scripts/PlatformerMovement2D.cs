using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlatformerMovement2D : MonoBehaviour
{
    public enum ControlType { Keyboard, Manual }
    [Header("Control")]
    public ControlType controlType = ControlType.Keyboard;

    [Header("Moving")]
    public float moveSpeed = 2.0f;
    [Range(0, 1)]
    public float horizontalDamping = 0.5f;
    public float maxMoveSpeed = 4.0f;
    public float stopSpeed = 0.8f;

    [Header("Jumping")]
    public float jumpForce = 400.0f;
    public float jumpGravityScale = 1f;
    public float fallGravityScale = 1.8f;
    [Range(0, 1)]
    public float cutJumpHeight = 1.0f;
    [Range(0, .5f)]
    public float jumpRememberTime = 0.15f;
    [Range(0, .5f)]
    public float groundedRememberTime = 0.15f;

    [Header("Collision")]
    public GroundChecker2D groundChecker;

    
    
    private Rigidbody2D rb;
    private float jumpRemember = 0.0f;
    private float groundedRemember = 0.0f;
    private float horizontalInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (groundChecker == null)
        {
            Debug.LogError("No Ground Checker assigned on " + gameObject.name);
            enabled = false;
        }
    }

    void Update()
    {

        groundedRemember -= Time.deltaTime;
        jumpRemember -= Time.deltaTime;

        if (groundChecker.grounded)
        {
            groundedRemember = groundedRememberTime;
        }

        if (controlType == ControlType.Keyboard)
        {

            horizontalInput = Input.GetAxisRaw("Horizontal");
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                jumpRemember = jumpRememberTime;
            }
            if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
            {
                if (rb.velocity.y > 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * cutJumpHeight);
                }
            }
        }
            
        if (groundedRemember > 0 && jumpRemember > 0)
        {
            groundedRemember = 0;
            jumpRemember = 0;
            Jump();
        }
    }

    void FixedUpdate()
    {
        if (rb.velocity.y < 0)
            rb.gravityScale  = fallGravityScale;
        else 
            rb.gravityScale = jumpGravityScale;
        
        float hVelocity = rb.velocity.x;
        hVelocity += horizontalInput * moveSpeed;
        hVelocity *= Mathf.Pow(1f - horizontalDamping, Time.deltaTime * 10f);

        rb.velocity =  new Vector2(Mathf.Clamp(hVelocity, -maxMoveSpeed, maxMoveSpeed), rb.velocity.y);
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    public void ManualJump()
    {
        if (controlType == ControlType.Manual)
            jumpRemember = jumpRememberTime;
        else
        {
            Debug.LogError("Can't do ManualJump if control type is not set to manual!");
        }
    }

    public void ManualMove(float input)
    {
        if (controlType == ControlType.Manual)
            horizontalInput = input;
        else
        {
            Debug.LogError("Can't do ManualMove if control type is not set to manual!");
        }
    }

    public void ManualStopMove()
    {
        if (controlType == ControlType.Manual)
            horizontalInput = 0.0f;
        else
        {
            Debug.LogError("Can't do ManualStopMove if control type is not set to manual!");
        }
    }
}
