using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;

    [Header("Jump & Gravity Settings")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Mouse Look Settings")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float mouseSensitivity = 100f;

    [Header("Walk Head Bob Settings")]
    [Tooltip("Head bob frequency when walking")]
    [SerializeField] private float walkBobFrequency = 1.5f;
    [Tooltip("Horizontal bob amplitude when walking")]
    [SerializeField] private float walkBobHorizontalAmplitude = 0.05f;
    [Tooltip("Vertical bob amplitude when walking")]
    [SerializeField] private float walkBobVerticalAmplitude = 0.05f;
    [Tooltip("Head bob smooth when walking")]
    [SerializeField] private float walkBobSmooth = 5f;

    [Header("Sprint Head Bob Settings")]
    [Tooltip("Head bob frequency when sprinting")]
    [SerializeField] private float sprintBobFrequency = 3f;
    [Tooltip("Horizontal bob amplitude when sprinting")]
    [SerializeField] private float sprintBobHorizontalAmplitude = 0.1f;
    [Tooltip("Vertical bob amplitude when sprinting")]
    [SerializeField] private float sprintBobVerticalAmplitude = 0.1f;
    [Tooltip("Head bob smooth when sprinting")]
    [SerializeField] private float sprintBobSmooth = 2f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation;

    private Vector3 cameraStartLocalPos;
    private float bobTimer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraStartLocalPos = playerCamera.localPosition;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleHeadBob();
    }

    private void HandleMovement()
    {
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (input.sqrMagnitude > 1f) input.Normalize();

        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        Vector3 move = transform.TransformDirection(input) * speed;

        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        Vector3 finalMove = move + Vector3.up * velocity.y;
        controller.Move(finalMove * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleHeadBob()
    {
        Vector2 movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        float magnitude = movementInput.magnitude;

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        float frequency = isSprinting ? sprintBobFrequency : walkBobFrequency;
        float horizontalAmplitude = isSprinting ? sprintBobHorizontalAmplitude : walkBobHorizontalAmplitude;
        float verticalAmplitude   = isSprinting ? sprintBobVerticalAmplitude   : walkBobVerticalAmplitude;
        float smooth = isSprinting ? sprintBobSmooth : walkBobSmooth;

        Vector3 targetPos = cameraStartLocalPos;
        
        if (controller.isGrounded && magnitude > 0f)
        {
            bobTimer += Time.deltaTime * frequency * magnitude;
            float xOffset = Mathf.Cos(bobTimer) * horizontalAmplitude * magnitude;
            float yOffset = Mathf.Sin(bobTimer * 2f) * verticalAmplitude * magnitude;
            targetPos += new Vector3(xOffset, yOffset, 0f);
        }
        else
        {
            bobTimer = 0f;
        }

        playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, targetPos, Time.deltaTime * smooth);
    }
}