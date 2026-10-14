using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform cameraRoot;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 6.5f;

    [Tooltip("Qué capas cuentan como piso (ideal: solo 'Ground').")]
    [SerializeField] private LayerMask groundMask = ~0;

    [Tooltip("Distancia extra para considerar 'en piso'.")]
    [SerializeField] private float groundCheckExtra = 0.08f;

    [Tooltip("Ángulo máximo de superficie para contar como piso.")]
    [SerializeField] private float maxGroundAngle = 55f;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;

    [Header("Dash")]
    [SerializeField] private float dashForce = 12f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 1.5f;
    [SerializeField] private ParticleSystem dashParticles;

    private Rigidbody rb;
    private CapsuleCollider capsule;
    private float pitch;

    private bool isDashing;
    private float dashCooldownTimer;

    private PlayerDebuffController debuff;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        debuff = GetComponent<PlayerDebuffController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Look();

        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space))
            Jump();

        if (Input.GetKeyDown(KeyCode.LeftShift))
            TryDash();

        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (!isDashing)
            Move();
    }

    void Look()
    {
        float mx = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float my = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        if (Mathf.Abs(mx) < 0.02f) mx = 0f;
        if (Mathf.Abs(my) < 0.02f) my = 0f;

        if (Cursor.lockState != CursorLockMode.Locked) return;

        transform.Rotate(0f, mx, 0f);

        pitch -= my;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        if (cameraRoot)
            cameraRoot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 dir = (transform.right * x + transform.forward * z).normalized;

        float speedMult = debuff ? debuff.SpeedMultiplier : 1f;
        float finalSpeed = moveSpeed * speedMult;

        Vector3 target = dir * finalSpeed;
        rb.velocity = new Vector3(target.x, rb.velocity.y, target.z);
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    bool IsGrounded()
    {
        // Calculamos un spherecast desde "casi abajo" del capsule hacia abajo
        float radius = capsule.radius * 0.95f;

        // Punto desde donde casteamos: centro del capsule
        Vector3 centerWorld = transform.TransformPoint(capsule.center);

        // Distancia desde el centro al "fondo" del capsule (aprox)
        float halfHeight = Mathf.Max(capsule.height * 0.5f, radius);
        float castDist = (halfHeight - radius) + groundCheckExtra;

        // Origen un poquito arriba para evitar que empiece ya penetrando el suelo
        Vector3 origin = centerWorld + Vector3.up * 0.02f;

        if (Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit hit, castDist, groundMask, QueryTriggerInteraction.Ignore))
        {
            // Que no cuente paredes: chequeo de ángulo de la normal
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            return angle <= maxGroundAngle;
        }

        return false;
    }

    void TryDash()
    {
        if (debuff && debuff.DashBlocked)
            return;

        if (isDashing) return;
        if (dashCooldownTimer > 0f) return;

        Vector3 inputDir = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0f,
            Input.GetAxisRaw("Vertical")
        );

        Vector3 dashDir = inputDir.magnitude > 0.1f
            ? (transform.right * inputDir.x + transform.forward * inputDir.z).normalized
            : transform.forward;

        StartCoroutine(DashRoutine(dashDir));
    }

    IEnumerator DashRoutine(Vector3 dir)
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;

        rb.velocity = Vector3.zero;
        rb.AddForce(dir * dashForce, ForceMode.Impulse);

        if (dashParticles) dashParticles.Play();

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
    }
}




