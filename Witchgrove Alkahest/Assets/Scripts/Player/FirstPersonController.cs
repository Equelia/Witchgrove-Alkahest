// FirstPersonController.cs
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
    [SerializeField] private CinemachineInputAxisController cCam;
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
    
    [Header("Tutorial Settings")]
    [SerializeField] private TutorialManager tutorialManager;

    private CharacterController controller;
    private Vector3 velocity; // vertical velocity
    private Vector3 horizontalVelocity;  // horizontal movement stored between frames
    private bool previousGrounded;
    private bool jumpRequested;

    private Vector3 headStartLocalPos;
    private float bobTimer;
    private float shakeTimer;
    private float shakeDuration;
    private float shakeAmplitude;
    
    private bool isLaunched = false;
    private Vector3 launchVelocity;
    private bool allowAirControlDuringLaunch = false;
    
    private Vector3 contactNormal = Vector3.up;

    // Expose inventory state so FootStepController can check it
    public bool InventoryOpen => inventoryPanel.activeSelf; // Modified: added public property

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
        bool invOpen = inventoryPanel.activeSelf || tutorialManager.IsTutorialActive();
        

        // Removed disabling virtual camera to prevent jitter on reopen
        // cCam.enabled = !invOpen; // Modified: this line was removed
        
        if (invOpen)
        {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                cCam.enabled = false;
            }

            if (controller.isGrounded)
            {
                horizontalVelocity = Vector3.zero;
                velocity.y = -2f;
            }
            else
            {
                velocity.y += gravity * Time.deltaTime;
            }

            Vector3 finalMove = horizontalVelocity + Vector3.up * velocity.y;
            controller.Move(finalMove * Time.deltaTime);

            return;
        }




        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            cCam.enabled = true;
        }

        HandleHeadBob();

        if (Input.GetButtonDown("Jump"))
            jumpRequested = true;

        if (shakeTimer > 0f)
            shakeTimer -= Time.deltaTime;

        HandleMovement();
        HandlePlayerRotation();
    }
    
    public void AddExternalForce(Vector3 force, bool allowAirControl = false)
    {
        isLaunched = true;
        launchVelocity = force;
        allowAirControlDuringLaunch = allowAirControl;
        controller.Move(Vector3.zero); // обнулим старое движение
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        contactNormal = hit.normal;
    }

    private void HandleMovement()
    {
        bool isGrounded = controller.isGrounded;
        
        if (isLaunched)
        {
            // Управление в воздухе, если включено
            if (allowAirControlDuringLaunch)
            {
                Vector3 _rawInput = new Vector3(
                    Input.GetAxisRaw("Horizontal"),
                    0f,
                    Input.GetAxisRaw("Vertical")
                );
                Vector3 inputDir = _rawInput.sqrMagnitude > 0f ? _rawInput.normalized : Vector3.zero;

                Transform cam = Camera.main.transform;
                Vector3 forward = cam.forward; forward.y = 0f; forward.Normalize();
                Vector3 _right = cam.right;     _right.y = 0f;   _right.Normalize();

                float speed = walkSpeed;
                Vector3 moveDir = forward * inputDir.z + _right * inputDir.x;
                launchVelocity += moveDir * airControlAcceleration * Time.deltaTime;
            }

            // Применяем гравитацию и двигаем
            launchVelocity += Physics.gravity * Time.deltaTime;
            controller.Move(launchVelocity * Time.deltaTime);

            if (isGrounded && launchVelocity.y < 0f)
            {
                isLaunched = false;
                launchVelocity = Vector3.zero;
            }

            return;
        }


    if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        Vector3 rawInput = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0f,
            Input.GetAxisRaw("Vertical")
        );
        Vector3 _inputDir = rawInput.sqrMagnitude > 0f ? rawInput.normalized : Vector3.zero;

        Transform _cam = Camera.main.transform;
        Vector3 _forward = _cam.forward; _forward.y = 0f; _forward.Normalize();
        Vector3 right   = _cam.right;   right.y   = 0f; right.Normalize();

        if (isGrounded)
        {
            float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
            horizontalVelocity = (_forward * _inputDir.z + right * _inputDir.x) * speed;
        }
        else
        {
            if (_inputDir != Vector3.zero)
            {
                float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
                Vector3 targetVel = (_forward * _inputDir.z + right * _inputDir.x) * targetSpeed;
                horizontalVelocity = Vector3.MoveTowards(
                    horizontalVelocity,
                    targetVel,
                    airControlAcceleration * Time.deltaTime
                );
            }
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

        velocity.y += gravity * Time.deltaTime;
        Vector3 finalMove = horizontalVelocity + Vector3.up * velocity.y;
        controller.Move(finalMove * Time.deltaTime);

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
        float magnitude = Mathf.Clamp01(movementInput.magnitude);
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
