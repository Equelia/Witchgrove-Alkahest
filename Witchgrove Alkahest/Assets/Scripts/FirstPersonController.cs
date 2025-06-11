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

    private float yawSmoothTime = 0.001f;
    private float pitchSmoothTime = 0.001f;

    private CharacterController controller;
    private Vector3 velocity;
    private float currentYaw;
    private float yawVelocity;
    private float currentPitch;
    private float pitchVelocity;
    private float xRotation;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();

    }

    private void HandleMovement()
    {
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (moveInput.sqrMagnitude > 1f) moveInput.Normalize();
        Vector3 move = transform.TransformDirection(moveInput) * speed;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        Vector3 finalMove = move + Vector3.up * velocity.y;
        controller.Move(finalMove * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        float targetYaw = transform.eulerAngles.y + mouseX;
        float targetPitch = currentPitch - mouseY;

        currentYaw = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetYaw, ref yawVelocity, yawSmoothTime);
        transform.rotation = Quaternion.Euler(0f, currentYaw, 0f);

        currentPitch = Mathf.SmoothDamp(currentPitch, targetPitch, ref pitchVelocity, pitchSmoothTime);
        xRotation = Mathf.Clamp(currentPitch, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
