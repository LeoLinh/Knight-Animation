using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Public Variables
    public float moveSpeed;
    public float jumpForce;
    public float dashSpeed;
    public float dashDuration;
    public float strikeSpeed;
    public float strikeDuration;
    public Transform groundCheckPoint;
    public float groundCheckRadius;
    public LayerMask WhatIsGround;

    // Private Variables
    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private bool isCrouching;
    private bool isBlocking;
    private bool isDashing;
    private bool isStriking;
    private bool isJumpAttacking; // Biến này để kiểm tra trạng thái JumpAttack

    void Start()
    {
        // Get Components
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Main Update Loop
        GroundCheck();
        HandleInput();
        Flip();
        UpdateAnimations();
    }

    private void GroundCheck()
    {
        // Check if the player is on the ground
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, WhatIsGround);

        // Reset trạng thái JumpAttack khi nhân vật chạm đất
        if (isGrounded)
        {
            isJumpAttacking = false;
            anim.ResetTrigger("JumpAttack"); // Đặt lại Trigger khi chạm đất
        }
    }

    private void HandleInput()
    {
        // Handle JumpAttack input
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isGrounded)
        {
            anim.SetTrigger("JumpAttack");
        }

        // Handle movement input
        if (!isCrouching && !isBlocking && !isDashing && !isStriking && !isJumpAttacking)
        {
            rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * moveSpeed, rb.velocity.y);
        }

        // Handle jump input
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Handle crouch input
        if (Input.GetKeyDown(KeyCode.S))
        {
            isCrouching = true;
            anim.SetBool("isCrouching", true);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            isCrouching = false;
            anim.SetBool("isCrouching", false);
        }

        // Handle attack input
        if (Input.GetKeyDown(KeyCode.Mouse0) && isGrounded)
        {
            anim.SetTrigger("isAttacking");
        }

        // Handle block input
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            anim.SetBool("isBlocking", true);
            isBlocking = true;
        }
        if (Input.GetKeyUp(KeyCode.Mouse2))
        {
            anim.SetBool("isBlocking", false);
            isBlocking = false;
        }

        // Handle dash input
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            StartCoroutine(Dash());
        }

        // Handle strike input
        if (Input.GetKeyDown(KeyCode.E) && !isStriking)
        {
            StartCoroutine(Strike());
        }

        // Handle die input
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            anim.SetTrigger("Die");
        }

        // Handle Dizzy input
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            anim.SetBool("Dizzy", true);
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            anim.SetBool("Dizzy", false);
        }

        // Handle hurt input
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            anim.SetTrigger("Hurt");
        }

        // Handle win input
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            anim.SetTrigger("Win");
        }

    }

    private void UpdateAnimations()
    {
        // Update animator parameters
        anim.SetFloat("isWalking", Mathf.Abs(rb.velocity.x));
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("ySpeed", rb.velocity.y);
    }

    private void Flip()
    {
        // Flip the player sprite based on movement direction
        if (rb.velocity.x > 0)
        {
            transform.localScale = Vector3.one;
        }
        else if (rb.velocity.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void Jump()
    {
        // Apply a vertical force to make the player jump
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        anim.SetTrigger("Dash");

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        float dashDirection = transform.localScale.x;
        rb.velocity = new Vector2(dashDirection * dashSpeed, 0);

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = Vector2.zero;
        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    private IEnumerator Strike()
    {
        isStriking = true;
        anim.SetTrigger("Strike");

        float strikeDirection = transform.localScale.x;
        rb.velocity = new Vector2(strikeDirection * strikeSpeed, rb.velocity.y);

        yield return new WaitForSeconds(strikeDuration);

        rb.velocity = Vector2.zero;
        isStriking = false;
    }
}
