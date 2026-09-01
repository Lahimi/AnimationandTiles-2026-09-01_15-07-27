using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f; // NEW: jump force
    public Transform groundCheck;  // NEW: empty GameObject at player's feet
    public float groundCheckRadius = 0.2f; // NEW: radius for ground detection
    public LayerMask groundLayer;  // NEW: layer for ground objects

    private Rigidbody2D rb;
    private PlayerInput playerInput;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded; // NEW: track if player is on ground

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        // Horizontal movement
        rb.linearVelocity = new Vector2(playerInput.moveInput * speed, rb.linearVelocity.y);
        
        // Ground check (do this in FixedUpdate for physics consistency)
        CheckGrounded();
    }

    void Update()
    {
        // Walking animation
        animator.SetBool("isWalking", playerInput.moveInput != 0);
        
        // Jumping animation - UPDATE: isJumping parameter based on vertical velocity and grounded state
        // We're not in the air if grounded OR if vertical velocity is very small
        bool shouldBeJumping = !isGrounded && rb.linearVelocity.y > 0.1f;
        animator.SetBool("isJumping", shouldBeJumping);
        
        // Handle jump input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // Flip sprite based on movement direction
        if (playerInput.moveInput > 0)
            spriteRenderer.flipX = false; // facing right
        else if (playerInput.moveInput < 0)
            spriteRenderer.flipX = true;  // facing left
    }

    // NEW: jump method
    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        // Optionally trigger jump animation immediately
        animator.SetTrigger("jump"); // If you have a jump trigger
    }

    // NEW: ground check method
    void CheckGrounded()
    {
        // Check if groundCheck is assigned
        if (groundCheck != null)
        {
            // Draw circle at groundCheck position and check if it overlaps with ground layer
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
        else
        {
            // Fallback: check using collider bounds
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                Vector2 groundCheckPos = new Vector2(transform.position.x, col.bounds.min.y + 0.05f);
                isGrounded = Physics2D.OverlapCircle(groundCheckPos, groundCheckRadius, groundLayer);
            }
        }
    }

    // NEW: visualize ground check in editor
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        else
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                Gizmos.color = Color.red;
                Vector2 groundCheckPos = new Vector2(transform.position.x, col.bounds.min.y + 0.05f);
                Gizmos.DrawWireSphere(groundCheckPos, groundCheckRadius);
            }
        }
    }
}