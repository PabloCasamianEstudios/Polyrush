using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float jumpForce = 5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Cameras")]
    public Camera camFPS;
    public Camera camTPS;
    public Camera camTopDown;

    public float camSensibility = 20f;
    private float xRotation = 0f;

    private Rigidbody rb;

    // suelo
    private bool isGrounded;
    private bool doubleJumpAvailable = true;
    private bool wasGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ActivateCamera(camFPS);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // movimiento
        HandleMovementInput();

        // salto y aire
        CheckGrounded();
        HandleJump();

        // cambio y manejo de cámara
        HandleCameraSwitch();
        HandleLook();
    }

    // -------------------- MOVIMIENTO --------------------
    private void HandleMovementInput()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        Vector3 newPos = rb.position + move * moveSpeed * Time.deltaTime;

        rb.MovePosition(newPos);
    }

    // -------------------- GROUND CHECK --------------------
    private void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded)
            doubleJumpAvailable = true; // reset
    }

    // -------------------- SALTO --------------------
    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                PerformJump();
            }
            else if (doubleJumpAvailable)
            {
                PerformJump();
                doubleJumpAvailable = false;
            }
        }
    }

    private void PerformJump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // -------------------- CAMBIAR CAMARAS --------------------
    private void HandleCameraSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ActivateCamera(camFPS);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ActivateCamera(camTPS);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ActivateCamera(camTopDown);
    }

    private void ActivateCamera(Camera cam)
    {
        camFPS.enabled = cam == camFPS;
        camTPS.enabled = cam == camTPS;
        camTopDown.enabled = cam == camTopDown;
    }

    // -------------------- ROTACIÓN CAMARA --------------------
    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * camSensibility;
        transform.Rotate(Vector3.up * mouseX);

        // FIRST PERSON
        if (camFPS.enabled)
        {
            float mouseY = Input.GetAxis("Mouse Y") * camSensibility;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -60f, 60f);
            camFPS.transform.localEulerAngles = new Vector3(xRotation, 0f, 0f);
        }

        // THIRD PERSON
        if (camTPS.enabled)
        {
            float mouseY = Input.GetAxis("Mouse Y") * camSensibility;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -5f, 45f);

            // Smooth con Lerp
            float smoothRotation = Mathf.Lerp(camTPS.transform.localEulerAngles.x, xRotation, Time.deltaTime * 10f);
            camTPS.transform.localEulerAngles = new Vector3(smoothRotation, 0f, 0f);
        }

        // TOP DOWN no necesita rotación
    }
}