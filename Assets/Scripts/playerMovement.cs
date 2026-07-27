using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Input Actions")]
    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction dashAction;

    [Header("Movement Settings")]
    public float speed = 10f;
    public float jumpForce = 14f;
    public float fastFallSpeed = 20f;

    [Header("Acceleration & Deceleration")]
    public float acceleration = 50f;
    public float deceleration = 60f;

    [Header("Dash Settings")]
    public float dashForce = 24f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing;
    private float dashCooldownTimer;

    [Header("Variable Jump Height")]
    [Range(0f, 1f)] public float jumpCutMultiplier = 0.4f;

    [Header("Coyote Time")]
    public float coyoteTimeDuration = 0.15f;
    private float coyoteTimeCounter;

    [Header("Wall Jump Settings")]
    public float wallSlideSpeed = 2f;
    public Vector2 wallJumpForce = new Vector2(12f, 15f);
    public float wallJumpDuration = 0.15f;
    private bool isWallSliding;
    private float wallJumpTimeLeft;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.4f, 0.1f);
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Wall Detection")]
    public Transform wallCheck;
    public Vector2 wallCheckSize = new Vector2(0.1f, 0.6f);
    public LayerMask wallLayer;
    private bool isTouchingWall;

    [Header("One Way Platform Settings")]
    public LayerMask oneWayLayer;
    private Collider2D playerCollider;
    private bool isDroppingThroughPlatform;

    private Animator anim;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float facingDirection = 1f;
    private float originalGravity;

    private void OnEnable()
    {
        moveAction.Enable();

        jumpAction.started += OnJumpStarted;
        jumpAction.canceled += OnJumpCanceled;
        jumpAction.Enable();

        dashAction.started += OnDashStarted;
        dashAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();

        jumpAction.started -= OnJumpStarted;
        jumpAction.canceled -= OnJumpCanceled;
        jumpAction.Disable();

        dashAction.started -= OnDashStarted;
        dashAction.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        originalGravity = rb.gravityScale;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        UpdateAnimator();

        if (isDashing) return;

        moveInput = moveAction.ReadValue<Vector2>();

        // Timer Wall Jump
        if (wallJumpTimeLeft > 0)
        {
            wallJumpTimeLeft -= Time.deltaTime;
        }

        // Timer Cooldown Dash (Bisa reset di Ground atau Wall)
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0 && (isGrounded || isTouchingWall))
            {
                canDash = true;
            }
        }

        // Memutar arah karakter
        if (wallJumpTimeLeft <= 0 && moveInput.x != 0)
        {
            float newDirection = Mathf.Sign(moveInput.x);
            if (newDirection != facingDirection) 
            { 
                Flip(); 
            }
        }

        // Deteksi Pijakan & Dinding
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer | oneWayLayer | wallLayer);
        isTouchingWall = Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0f, wallLayer);

        // Logika Grounding & Coyote Time
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTimeDuration;
            if (dashCooldownTimer <= 0) canDash = true;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Reset dash jika menyentuh dinding
        if (isTouchingWall && dashCooldownTimer <= 0)
        {
            canDash = true;
        }

        // Logika Wall Slide
        if (isTouchingWall && !isGrounded && rb.linearVelocity.y <= 0f)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void UpdateAnimator()
    {
        if (anim == null) return;

        anim.SetFloat("Speed", Mathf.Abs(moveInput.x));
        anim.SetFloat("VelocityY", rb.linearVelocity.y);
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetBool("IsWallSliding", isWallSliding);
        anim.SetBool("IsDashing", isDashing);
    }

    void FixedUpdate()
    {
        if (isDashing || wallJumpTimeLeft > 0) return;

        if (isWallSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
        }
        else
        {
            float targetSpeed = moveInput.x * speed;

            if (isGrounded)
            {
                float currentRate = (moveInput.x != 0) ? acceleration : deceleration;
                float newXVelocity = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, currentRate * Time.fixedDeltaTime);
                rb.linearVelocity = new Vector2(newXVelocity, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
            }

            // Fast Fall saat menekan bawah di udara (hanya jika tidak sedang proses turun platform)
            if (!isGrounded && moveInput.y < 0f && !isDroppingThroughPlatform)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -fastFallSpeed);
            }
        }
    }

    private void OnJumpStarted(InputAction.CallbackContext context)
    {
        if (isDashing) return;

        if (coyoteTimeCounter > 0f)
        {
            // Jika menekan tombol BAWAH (S / Panah Bawah) saat berada di atas One-Way Platform
            Collider2D oneWayPlatform = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, oneWayLayer);
            if (moveInput.y < 0f && oneWayPlatform != null)
            {
                StartCoroutine(DisableOneWayCollision(oneWayPlatform));
            }
            else
            {
                Jump();
            }
        }
        else if (isWallSliding)
        {
            WallJump();
        }
    }

    private IEnumerator DisableOneWayCollision(Collider2D platformCollider)
    {
        if (playerCollider == null) yield break;

        isDroppingThroughPlatform = true;
        
        // Beri dorongan kecil ke bawah agar karakter langsung keluar dari permukaan atas platform secara mulus
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -3f);

        Collider2D[] colliders = Physics2D.OverlapBoxAll(groundCheck.position, groundCheckSize, 0f, oneWayLayer);
        
        foreach (var col in colliders)
        {
            if (col != null) Physics2D.IgnoreCollision(playerCollider, col, true);
        }

        yield return new WaitForSeconds(0.3f);

        foreach (var col in colliders)
        {
            if (col != null) Physics2D.IgnoreCollision(playerCollider, col, false);
        }

        isDroppingThroughPlatform = false;
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        if (rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            coyoteTimeCounter = 0f;
        }
    }

    private void OnDashStarted(InputAction.CallbackContext context)
    {
        if (canDash && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        coyoteTimeCounter = 0f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayJumpSFX();
        }
    }

    void WallJump()
    {
        isWallSliding = false;
        wallJumpTimeLeft = wallJumpDuration;
        float kickDirection = -facingDirection;

        rb.linearVelocity = new Vector2(kickDirection * wallJumpForce.x, wallJumpForce.y);

        Flip();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayJumpSFX();
        }
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDashSFX();
        }

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero; // Menghapus momentum lama agar dash lurus

        Vector2 dashDirection = Vector2.zero;

        if (moveInput != Vector2.zero)
        {
            float dashX = moveInput.x != 0 ? Mathf.Sign(moveInput.x) : 0f;
            float dashY = moveInput.y != 0 ? Mathf.Sign(moveInput.y) : 0f;
            dashDirection = new Vector2(dashX, dashY).normalized;
        }

        if (dashDirection == Vector2.zero)
        {
            dashDirection = new Vector2(facingDirection, 0f);
        }

        rb.linearVelocity = dashDirection * dashForce;

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        
        if (dashDirection.y > 0)
        {
            rb.linearVelocity = new Vector2(dashDirection.x * speed, jumpForce * 0.5f);
        }
        else
        {
            rb.linearVelocity = new Vector2(dashDirection.x * speed, rb.linearVelocity.y);
        }

        if (!isGrounded && moveInput.y < 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -fastFallSpeed);
        }

        isDashing = false;
        dashCooldownTimer = dashCooldown;

        // Auto-reset jika dash berakhir di dinding
        if (isTouchingWall)
        {
            canDash = true;
        }
    }

    void Flip()
    {
        facingDirection *= -1;
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
        }
    }
}