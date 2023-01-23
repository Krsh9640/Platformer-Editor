using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogBehaviour : MonoBehaviour
{
    public bool facingRight;
    public LayerMask whatIsGround;

    public bool isGrounded = true;
    public bool isFalling;
    public bool isJumping;

    public float jumpForceX = 2f;
    public float jumpForceY = 4f;

    public float lastYPosition = 0;

    public enum Animations
    {
        Idle = 0,
        Jumping = 1,
        Falling = 2,
        Death = 3
    };

    public Animations currentAnim;

    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Animator anim;

    public float idleTime = 2f;
    public float currentIdleTime = 0;
    public bool isIdle = true;

    private GameState gameState;

    void Start()
    {
        lastYPosition = transform.position.y;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        gameState = GameObject.Find("Manager").GetComponent<GameState>();
    }

    void FixedUpdate()
    {
        if (gameState.isPlay == true)
        {
            if (isIdle == true)
            {
                currentIdleTime += Time.deltaTime;
                if (currentIdleTime >= idleTime)
                {
                    currentIdleTime = 0;
                    facingRight = !facingRight;
                    spriteRenderer.flipX = facingRight;
                    Jump();
                }
            }

            isGrounded = Physics2D.OverlapArea(new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f),
            new Vector2(transform.position.x + 0.5f, transform.position.y + 0.5f), whatIsGround);

            if (isGrounded == true && isJumping == false)
            {
                isFalling = false;
                isJumping = false;
                isIdle = true;
                ChangeAnimation(Animations.Idle);
            }
            else if (transform.position.y > lastYPosition && !isGrounded && !isIdle)
            {
                isJumping = true;
                isFalling = false;
                ChangeAnimation(Animations.Jumping);
            }
            else if (transform.position.y < lastYPosition && !isGrounded && !isIdle)
            {
                isJumping = false;
                isFalling = true;
                ChangeAnimation(Animations.Falling);
            }
            lastYPosition = transform.position.y;
        }
    }

    void Jump()
    {
        isJumping = true;
        isIdle = false;
        int direction = 0;

        if (facingRight == true)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }

        rb.velocity = new Vector2(jumpForceX * direction, jumpForceY);
    }

    public void ChangeAnimation(Animations newAnim)
    {
        if (currentAnim != newAnim)
        {
            currentAnim = newAnim;
            anim.SetInteger("state", (int)newAnim);
        }
    }
}
