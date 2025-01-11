using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    private float horizontal;
    private float speed = 14f;
    private float jumpingPower = 10f;
    private bool isFacingRight = true;

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 2f;
    private float dashingTime = 0.8f;
    private float dashingCooldown = 1f;

    private bool canDoubleJump = true;
    private int extraJumps = 1;

    private bool isClimbing;
    private float climbSpeed = 5f; // Týrmanma hýzý
    private float verticalInput;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer tr;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (horizontal != 0)
        {
            animator.SetInteger("AnimState1", 1);
        }
        else
        {
            animator.SetInteger("AnimState1", 0);
        }


        if (isDashing)
        {
            return;
        }

        horizontal = Input.GetAxisRaw("Horizontal");

        // Merdiven týrmanma kontrolü
        if (isClimbing)
        {
            verticalInput = Input.GetAxisRaw("Vertical");
            rb.velocity = new Vector2(rb.velocity.x, verticalInput * climbSpeed);
        }
        else // Normal zemin üzerinde kontrol
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (IsGrounded())
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                    extraJumps = 1; // Bir kere zýpladýk, ekstra zýplama hakký resetlendi.
                }
                else if (canDoubleJump && extraJumps > 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                    extraJumps--;
                }
            }

            if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            {
                StartCoroutine(Dash());
            }
        }

        Flip();
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            canDoubleJump = true;
            extraJumps = 1;
        }
        else if (collision.gameObject.CompareTag("Ladder"))
        {
            isClimbing = true;
            rb.gravityScale = 0f; // Yerçekimini kapat
            rb.velocity = Vector2.zero; // Hareketi sýfýrla
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ladder"))
        {
            isClimbing = false;
            rb.gravityScale = 1f; // Yerçekimini geri aç
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}