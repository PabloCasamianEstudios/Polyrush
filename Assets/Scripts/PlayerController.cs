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
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ActivateCamera(camFPS); // Empieza con FPS

        Cursor.lockState = CursorLockMode.Locked; // bloquearlo centro
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovementInput();
        HandleJump();
        HandleCameraSwitch();
        HandleLook();
    }

    private void HandleMovementInput()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        Vector3 newPos = rb.position + move * moveSpeed * Time.deltaTime;

        rb.MovePosition(newPos);
    }

    private void HandleJump()
    {
        isGrounded = false;
        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, groundDistance);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Ground"))
            {
                Debug.Log("suelo");
                isGrounded = true;
                break;
            }
        }

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

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

    // Lógica de todas las cámaras
    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * camSensibility;
        transform.Rotate(Vector3.up * mouseX);

        // FIRST PERSON
        if (camFPS.enabled)
        {
            float mouseY = Input.GetAxis("Mouse Y") * camSensibility;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -60f, 60f); // límite vertical
            camFPS.transform.localEulerAngles = new Vector3(xRotation, 0f, 0f);
        }

        // THIRD PERSON
        if (camTPS.enabled)
        {
            float mouseY = Input.GetAxis("Mouse Y") * camSensibility;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -5f, 45f); // límite vertical
            camTPS.transform.localEulerAngles = new Vector3(xRotation, 0f, 0f);
        }

        // para la top down tengo que ignorar el cursor para que no sea confuso, se mira hacia donde se mueve el player


        // para el pablo del futuro: suavizar third y top, y sacar la top del player

    }
}
