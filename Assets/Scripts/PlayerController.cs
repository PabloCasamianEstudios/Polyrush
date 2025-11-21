using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float jumpForce = 5f;
    public float friction = 1f;

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

    [Header("Dash Settings")]
    public float dashForce = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool isDashing = false;
    private bool canDash = true;
    private TrailRenderer dashTrail;
    public Material trailMaterial;

    [Header("Player Control")]
    public bool canMove = false;
    public bool canLook = true;

    private Rigidbody rb;

    // suelo
    private bool isGrounded;
    private bool doubleJumpAvailable = true;
    private bool wasGrounded;

    [Header("Rocket Launcher")]
    public RocketLauncher rocketLauncher;

    [Header("Points")]
    public int points = 0;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;

        // el trail del dash
        SetupDashTrail();



        ActivateCamera(camFPS);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // cambio y manejo de cámara
        HandleCameraSwitch();
        HandleLook();

        if (!canMove) return;

        // movimiento
        HandleMovementInput();

        // salto y aire
        CheckGrounded();
        HandleJump();

        // Dash
        HandleDash();

        if (Input.GetButtonDown("Fire1"))
        {
            rocketLauncher.Shoot();
        }
    }


    // -------------------- MOVIMIENTO --------------------

    private void HandleMovementInput()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        if (camTopDown.enabled) // TopDown - mov relativo a la cámara
        {
            Vector3 forward = camTopDown.transform.forward;
            Vector3 right = camTopDown.transform.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            move = forward * moveZ + right * moveX;

            if (move.magnitude > 0.1f) // Rotar personaje hacia la dirección de movimiento
            {
                Quaternion targetRotation = Quaternion.LookRotation(move.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }
        else // FPS Y TPS - mov relativo al player
        {
            move = transform.right * moveX + transform.forward * moveZ;
        }

        Vector3 newPos = rb.position + move * moveSpeed * Time.deltaTime;
        rb.MovePosition(newPos);

    }

    // -------------------- GROUND CHECK --------------------
    private void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && rb.linearVelocity.y <= 0) // no deslizarse al tocar suelo de nuevo
        {
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (horizontalVelocity.magnitude > moveSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
            }

            doubleJumpAvailable = true; // reset
        }
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

    // -------------------- DASH --------------------
    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.F) && canDash && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    private System.Collections.IEnumerator PerformDash()
    {
        isDashing = true;
        canDash = false;

        // Activar trail
        if (dashTrail != null)
            dashTrail.emitting = true;

        // dirección inicial del dash
        Vector3 dashDirection = transform.forward;

        rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);

        float timer = 0f;
        while (timer < dashDuration)
        {
            Vector3 currentVelocity = rb.linearVelocity;
            Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

            if (horizontalVelocity.magnitude < dashForce)
            {
                Vector3 dashMaintainForce = dashDirection * dashForce * 0.5f;
                rb.AddForce(dashMaintainForce * Time.deltaTime, ForceMode.VelocityChange);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        yield return new WaitForSeconds(0.2f);
        if (dashTrail != null)
            dashTrail.emitting = false;

        // cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // -------------------- DASH - TRAIL --------------------
    private void SetupDashTrail()
    {
        GameObject trailObject = new GameObject("DashTrail");
        trailObject.transform.SetParent(transform);
        trailObject.transform.localPosition = Vector3.zero;

        dashTrail = trailObject.AddComponent<TrailRenderer>();

        // trail
        dashTrail.time = 0.5f;
        dashTrail.startWidth = 0.7f;
        dashTrail.endWidth = 0.2f;
        dashTrail.minVertexDistance = 0.1f;

        if (trailMaterial != null)
        {
            dashTrail.material = trailMaterial;
        }

        // Config color
        Gradient gradient = new Gradient();
        gradient.colorKeys = new GradientColorKey[]
        {
        new GradientColorKey(Color.yellow, 0f),
        new GradientColorKey(new Color(1f, 0.8f, 0f), 1f)
        };
        gradient.alphaKeys = new GradientAlphaKey[]
        {
        new GradientAlphaKey(1f, 0f),
        new GradientAlphaKey(0f, 1f)
        };
        dashTrail.colorGradient = gradient;

        dashTrail.emitting = false;
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
        if (!canLook) return;
        // TOP DOWN no necesita mover la cam
        if (camTopDown.enabled) return;

        float mouseX = Input.GetAxis("Mouse X") * camSensibility;
        float mouseY = Input.GetAxis("Mouse Y") * camSensibility;

        // Rotación horizontal del jugador
        transform.Rotate(Vector3.up * mouseX);

        // FIRST PERSON y THIRD PERSON - Rotación vertical de la cámara
        if (camFPS.enabled)
        {
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -60f, 60f);
            camFPS.transform.localEulerAngles = new Vector3(xRotation, 0f, 0f);
        }

        if (camTPS.enabled)
        {
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -5f, 45f);

            Quaternion targetRotation = Quaternion.Euler(xRotation, 0f, 0f);
            camTPS.transform.localRotation = Quaternion.Slerp(camTPS.transform.localRotation, targetRotation, Time.deltaTime * 10f);
        }
    }


    // -------------------- EXPLOSION JUMP --------------------
    public void ApplyExplosionForce(Vector3 explosionPosition, float force, float radius)
    {
        Vector3 dir = rb.position - explosionPosition;
        float distance = dir.magnitude;

        if (distance > radius) return;

        float effect = 1f - (distance / radius);
        rb.AddForce(dir.normalized * force * effect, ForceMode.Impulse);
    }

    // -------------------- CONTROL --------------------

    public void SetMovementEnabled(bool isEnabled)
    {
        canMove = isEnabled;
    }

    public void SetLookEnabled(bool isEnabled)
    {
        canLook = isEnabled;
    }

    // -------------------- POINTS --------------------
    public void AddPoints(int amount)
    {
        points += amount;
        Debug.Log("Monedas: " + points);
    }

}