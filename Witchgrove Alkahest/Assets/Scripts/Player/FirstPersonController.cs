using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float minAirSpeed = 2f;                 
    [SerializeField] private float airControlAcceleration = 3f;       

    [Header("Slide Settings")]
    [SerializeField] private float slideSpeed = 5f;  // speed when sliding down slopes steeper than slopeLimit

    [Header("Jump & Gravity Settings")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("References")]
    [SerializeField] private Transform headTransform;
    [SerializeField] private CinemachineCamera cCam;
    [SerializeField] private GameObject inventoryPanel;

    [Header("Head Bob Settings")]
    [SerializeField] private float walkBobFrequency = 1.5f;
    [SerializeField] private float walkBobHorizontalAmplitude = 0.05f;
    [SerializeField] private float walkBobVerticalAmplitude = 0.05f;
    [SerializeField] private float walkBobSmooth = 5f;

    [SerializeField] private float sprintBobFrequency = 3f;
    [SerializeField] private float sprintBobHorizontalAmplitude = 0.1f;
    [SerializeField] private float sprintBobVerticalAmplitude = 0.1f;
    [SerializeField] private float sprintBobSmooth = 2f;

    [Header("Jump/Land Shake Settings")]
    [SerializeField] private float jumpShakeDuration = 0.2f;
    [SerializeField] private float jumpShakeAmplitude = 0.1f;
    [SerializeField] private float landShakeDuration = 0.3f;
    [SerializeField] private float landShakeAmplitude = 0.15f;

    private CharacterController controller;
    private Vector3 velocity;            // vertical velocity
    private Vector3 horizontalVelocity;  // horizontal movement stored between frames
    private bool previousGrounded;
    private bool jumpRequested;

    private Vector3 headStartLocalPos;
    private float bobTimer;
    private float shakeTimer;
    private float shakeDuration;
    private float shakeAmplitude;

    private Vector3 contactNormal = Vector3.up;  // stores the normal of the last surface we touched

    void Start()
    {
        controller = GetComponent<CharacterController>();
        headStartLocalPos = headTransform.localPosition;
        previousGrounded = controller.isGrounded;
        horizontalVelocity = Vector3.zero;  
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        bool invOpen = inventoryPanel.activeSelf;
        
        cCam.enabled = !invOpen;

        if (invOpen)
        {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            return;
        }
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        HandleHeadBob();

        if (Input.GetButtonDown("Jump"))
            jumpRequested = true;

        if (shakeTimer > 0f)
            shakeTimer -= Time.deltaTime;

        HandleMovement();
        HandlePlayerRotation();
    }

    // capture the normal of whatever we collide with
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        contactNormal = hit.normal;
    }

    private void HandleMovement()
    {
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        // if standing on too-steep slope, slide down and skip the rest of movement logic
        if (isGrounded)
        {
            float slopeAngle = Vector3.Angle(contactNormal, Vector3.up);
            if (slopeAngle > controller.slopeLimit)
            {
                Vector3 slideDir = new Vector3(contactNormal.x, -contactNormal.y, contactNormal.z);
                controller.Move(slideDir.normalized * slideSpeed * Time.deltaTime);
                return;
            }
        }

        // use raw input for instant respond/release
        Vector3 rawInput = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0f,
            Input.GetAxisRaw("Vertical")
        );
        Vector3 inputDir = rawInput.sqrMagnitude > 0f ? rawInput.normalized : Vector3.zero;

        // calculate camera-based axes
        Transform cam = Camera.main.transform;
        Vector3 forward = cam.forward; forward.y = 0f; forward.Normalize();
        Vector3 right   = cam.right;   right.y   = 0f; right.Normalize();

        if (isGrounded)
        {
            // update horizontal velocity on ground
            float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
            horizontalVelocity = (forward * inputDir.z + right * inputDir.x) * speed;
        }
        else
        {
            // IN AIR: apply directional control
            if (inputDir != Vector3.zero)
            {
                float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
                Vector3 targetVel = (forward * inputDir.z + right * inputDir.x) * targetSpeed;
                horizontalVelocity = Vector3.MoveTowards(
                    horizontalVelocity,
                    targetVel,
                    airControlAcceleration * Time.deltaTime
                );
            }
            // enforce minimal air speed if below threshold
            if (horizontalVelocity.magnitude < minAirSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * minAirSpeed;
            }
        }

        if (jumpRequested && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            TriggerShake(jumpShakeDuration, jumpShakeAmplitude);
        }
        jumpRequested = false;

        // apply gravity
        velocity.y += gravity * Time.deltaTime;

        // move character
        Vector3 finalMove = horizontalVelocity + Vector3.up * velocity.y;
        controller.Move(finalMove * Time.deltaTime);

        // if landing on a too-steep slope, reset horizontal speed to prevent climbing
        if (!previousGrounded && isGrounded)
        {
            float landAngle = Vector3.Angle(contactNormal, Vector3.up);
            if (landAngle > controller.slopeLimit)
                horizontalVelocity = Vector3.zero;
            TriggerShake(landShakeDuration, landShakeAmplitude);
        }

        previousGrounded = isGrounded;
    }

    private void HandlePlayerRotation()
    {
        Vector3 dir = Camera.main.transform.forward;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                10f * Time.deltaTime
            );
    }

    private void HandleHeadBob()
    {
        Vector2 movementInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );
        float magnitude = movementInput.magnitude;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        float frequency = isSprinting ? sprintBobFrequency : walkBobFrequency;
        float hAmp = isSprinting ? sprintBobHorizontalAmplitude : walkBobHorizontalAmplitude;
        float vAmp = isSprinting ? sprintBobVerticalAmplitude : walkBobVerticalAmplitude;
        float smooth = isSprinting ? sprintBobSmooth : walkBobSmooth;

        Vector3 targetPos = headStartLocalPos;

        if (controller.isGrounded && magnitude > 0f)
        {
            bobTimer += Time.deltaTime * frequency * magnitude;
            float xOffset = Mathf.Cos(bobTimer) * hAmp * magnitude;
            float yOffset = Mathf.Sin(bobTimer * 2f) * vAmp * magnitude;
            targetPos += new Vector3(xOffset, yOffset, 0f);
        }
        else bobTimer = 0f;

        if (shakeTimer > 0f)
        {
            float elapsed = shakeDuration - shakeTimer;
            float progress = Mathf.Clamp01(elapsed / shakeDuration);
            float damper = 1f - progress;
            float shakeFreq = Mathf.PI * 4f;
            float xShake = Mathf.Sin(elapsed * shakeFreq) * shakeAmplitude * damper;
            float yShake = Mathf.Sin(elapsed * shakeFreq * 1.5f) * shakeAmplitude * damper;
            targetPos += new Vector3(xShake, yShake, 0f);
        }

        headTransform.localPosition = Vector3.Lerp(
            headTransform.localPosition,
            targetPos,
            Time.deltaTime * smooth
        );
    }

    private void TriggerShake(float duration, float amplitude)
    {
        shakeDuration = duration;
        shakeTimer = duration;
        shakeAmplitude = amplitude;
    }
}
