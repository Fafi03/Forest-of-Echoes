using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.25f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // A/D or arrow keys
        moveInput = Input.GetAxisRaw("Horizontal");

        // Flip sprite
        if (moveInput > 0 && !facingRight)
            Flip();
        else if (moveInput < 0 && facingRight)
            Flip();

        
        // Check if on ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius);
        Debug.Log("isGrounded = " + isGrounded);

        // Jump
        // Space, W or UpArrow to jump
if (isGrounded && (Input.GetKeyDown(KeyCode.Space) ||
                   Input.GetKeyDown(KeyCode.W) ||
                   Input.GetKeyDown(KeyCode.UpArrow)))
{
    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
}

    }

    void FixedUpdate()
    {
        // Horizontal movement
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
