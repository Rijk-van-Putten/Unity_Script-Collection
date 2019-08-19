using UnityEngine;
 
[RequireComponent(typeof(CharacterController))]
public class FPCharacterMovement : MonoBehaviour
{
    [Header("Moving")]
    public float walkSpeed = 4.0f;
    public float runSpeed = 6.0f;
    public bool toggleRun = false;

    [Header("Jumping")]
    public float jumpSpeed = 8.0f;
    public bool airControl = false;
    public float gravity = 17.0f;
    [Tooltip("Units that player can fall before a falling function is run. To disable, type \"infinity\" in the inspector.")]
    public float fallingThreshold = 10.0f;

    [Header("Sliding")]
    public bool slideWhenOverSlopeLimit = true;
    public float slideSpeed = 1.0f;
    [Tooltip("If checked and the player is on an object tagged \"Slide\", he will slide down it regardless of the slope limit.")]
    public bool slideOnTaggedObjects = false;

    [Header("Rotating")]
    public Transform cameraDirection = null;
    public bool keepRotationZero = false;

    [Header("Anti-Cheat")]
    [Tooltip("Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast.")]
    public float antiBumpFactor = .75f;
    [Tooltip("Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping.")]
    public int antiBunnyHopFactor = 1;
          
    [Header("Fall Damage")]
    public bool dealFallDamage = false;
    public float fallDamageMultiplier = 3.0f;
    public HealthController heathController = null;

    [Header("Pushing Objects")]
    public float pushPower = 2.0f;

    [Header("Animations")]
    public Animator[] animators;
    public string parameterName = "Blend";
    public float dampTime = .1f;

    [Header("Player Settings")]
    public bool usePlayerSettings = true;

    private Vector3 moveDirection = Vector3.zero;
    private bool grounded = false;
    private CharacterController controller;
    private Transform transf;
    private float speed;
    private RaycastHit hit;
    private float fallStartLevel;
    private bool falling;
    private float slideLimit;
    private float rayDistance;
    private Vector3 contactPoint;
    private bool playerControl = false;
    private int jumpTimer;

    void Start()
    {
        transf = GetComponent<Transform>();
        controller = GetComponent<CharacterController>();
 
        speed = walkSpeed;
        rayDistance = controller.height * .5f + controller.radius;
        slideLimit = controller.slopeLimit - .1f;
        jumpTimer = antiBunnyHopFactor;
    }
  
    void Update()
    {
        if (!CursorController.IsInGame)
            return;

        if (toggleRun && grounded && Input.GetButtonDown("Run"))
        {
            speed = (speed == walkSpeed ? runSpeed : walkSpeed);
        }
    }
  
    void FixedUpdate()
    {
        float inputX;
        float inputY;
        if (usePlayerSettings)
        {
            inputX = InputManager.GetHorizontalRawAxis();
            inputY = InputManager.GetVerticalRawAxis();
        }
        else
        {
            inputX = Input.GetAxis("Horizontal");
            inputY = Input.GetAxis("Vertical");
        }

        //TODO: TEMP ANIMATION FIX REALY BAD CODE!!
        if (inputX == 0 && inputY == 0)
        {
            foreach (Animator animator in animators) { animator.SetFloat(parameterName, 0.0f, dampTime, Time.deltaTime); }
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            foreach (Animator animator in animators) { animator.SetFloat(parameterName, 1.0f, dampTime, Time.deltaTime); }
        }
        else
        {
            foreach (Animator animator in animators) { animator.SetFloat(parameterName, 0.5f, dampTime, Time.deltaTime); }
        }

        // If both horizontal and vertical are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move speed
        float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f) ? .7071f : 1.0f;
 
        if (grounded)
        {
            bool sliding = false;
            // See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
            // because that interferes with step climbing amongst other annoyances
            if (Physics.Raycast(transf.position, -Vector3.up, out hit, rayDistance))
            {
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                {
                    sliding = true;
                }
            }
            // However, just raycasting straight down from the center can fail when on steep slopes
            // So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
            else
            {
                Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                {
                    sliding = true;
                }
            }
 
            // If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
            if (falling)
            {
                falling = false;
                if (transf.position.y < fallStartLevel - fallingThreshold)
                {
                    OnFell(fallStartLevel - transf.position.y);
                }
            }
 
            // If running isn't on a toggle, then use the appropriate speed depending on whether the run button is down
            if (!toggleRun)
            {
                speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            }
 
            // If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
            if ((sliding && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == "Slide"))
            {
                Vector3 hitNormal = hit.normal;
                moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                Vector3.OrthoNormalize(ref hitNormal, ref moveDirection);
                moveDirection *= slideSpeed;
                playerControl = false;
            }
            // Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
            else
            {
                moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
                moveDirection = transf.TransformDirection(moveDirection) * speed;
                playerControl = true;
            }
 
            // Jump! But only if the jump button has been released and player has been grounded for a given number of frames
            if (!Input.GetButton("Jump"))
            {
                jumpTimer++;
            }
            else if (jumpTimer >= antiBunnyHopFactor)
            {
                moveDirection.y = jumpSpeed;
                jumpTimer = 0;
            }
        }
        else
        {
            // If we stepped over a cliff or something, set the height at which we started falling
            if (!falling)
            {
                falling = true;
                fallStartLevel = transf.position.y;
            }
 
            // If air control is allowed, check movement but don't touch the y component
            if (airControl && playerControl)
            {
                moveDirection.x = inputX * speed * inputModifyFactor;
                moveDirection.z = inputY * speed * inputModifyFactor;
                moveDirection = transf.TransformDirection(moveDirection);
            }
        }
        if (!CursorController.IsInGame)
            return;
        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;
        
        // Move the controller, and set grounded true or false depending on whether we're standing on something
        Quaternion actualDir = Quaternion.Euler(0, cameraDirection.eulerAngles.y, 0);
        grounded = (controller.Move(actualDir * moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;

        if (keepRotationZero)
            transform.eulerAngles = Vector3.zero;
    }
  
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        contactPoint = hit.point;
        
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
        {
            return;
        }

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3)
        {
            return;
        }

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.

        // Apply the push
        body.velocity = pushDir * pushPower;
    }
    
    void OnFell(float fallDistance)
    {
        if (dealFallDamage)
        {
            heathController.DealDamage(fallDistance * fallDamageMultiplier);
        }
    }
}