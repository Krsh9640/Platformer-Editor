using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;
    public GameObject manager;
    private GameState gameState;
    private SaveHandler saveHandler;

    public float horizontalMove = 0f;
    public float runSpeed = 100f;

    public bool jump = false;
    public bool isGrounded = true;

    public int playerHealth = 5;
    public int currentHealth;

    bool isInvincible = false;

    public GameObject blink;
    public TMP_Text healthText;

    public Rigidbody2D rb;

    private float jumpTime;
    private float jumpTimeCounter;
    private bool isJumping = false;
    private float jumpForce = 10f;

    public Vector3 startingSpawnPos;

    private AudioSource audioSource;
    public AudioClip jumpSound, deathSound;

    public GameObject DeathInterval;
    public TMP_Text DeathCounter;
    private int DeathCounterVal;

    void Awake()
    {
        currentHealth = playerHealth;
        rb = gameObject.GetComponent<Rigidbody2D>();
        manager = GameObject.Find("Manager");
        gameState = manager.GetComponent<GameState>();
        audioSource = manager.GetComponent<AudioSource>();
        saveHandler = manager.GetComponent<SaveHandler>();

        startingSpawnPos = this.gameObject.transform.position;
    }

    void Update()
    {
        if (healthText.text != currentHealth.ToString())
        {
            healthText.text = currentHealth.ToString();
        }

        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump") && isGrounded == true)
        {
            isGrounded = false;
            isJumping = true;
            jumpTimeCounter = jumpTime;
            jump = true;
            animator.SetBool("isJumping", true);
            audioSource.PlayOneShot(jumpSound, 0.5f);
        }

        if (isJumping == true)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            isJumping = false;
        }

        DeathCheck();

        if (gameState.isPlay == false)
        {
            DisableBlink();
            currentHealth = 5;
            this.gameObject.transform.position = startingSpawnPos;
        }
    }

    public void OnLanding()
    {
        animator.SetBool("isJumping", false);
    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Platform") || collision.collider.gameObject.layer == LayerMask.NameToLayer("Prefab"))
        {
            if (collision.collider.gameObject.name != "Spring(Clone)")
            {
                isGrounded = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy" && isInvincible == false && isGrounded == true)
        {
            SetInvincible(3);
            Invoke("EnableBlink", 0f);
            Invoke("DisableBlink", 0.1f);
        }
    }

    public void SetInvincible(float invincibleTime)
    {
        if (isInvincible == false)
        {
            currentHealth--;
        }
        isInvincible = true;

        StartCoroutine(COSetInvincible(invincibleTime));
    }

    void SetDamageable()
    {
        isInvincible = false;
    }

    public IEnumerator COSetInvincible(float invincibleTime)
    {
        yield return new WaitForSeconds(invincibleTime);
        SetDamageable();
    }

    private void EnableBlink()
    {
        blink.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void DisableBlink()
    {
        blink.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void DeathCheck()
    {
        if (this.gameObject.tag == "Player")
        {
            if (this.gameObject.transform.position.y <= -4)
            {
                if (gameState.isPlay == true && gameState.FromEditMode == true)
                {
                    gameState.StopPlaying();

                    this.gameObject.transform.position = startingSpawnPos;
                }
                else if (gameState.isPlay == true && gameState.FromPlayMode == true)
                {
                    if (this.gameObject.activeInHierarchy)
                    {
                        StartCoroutine(DeathOrder());
                        audioSource.PlayOneShot(deathSound, 0.5f);

                        saveHandler.OnLoad();
                    }
                }
            }
        }
    }

    public IEnumerator DeathOrder()
    {
        audioSource.PlayOneShot(deathSound, 0.5f);

        DeathInterval.SetActive(true);

        if (DeathInterval.activeInHierarchy)
        {
            DeathCounterVal++;
            DeathCounter.text = DeathCounterVal.ToString();

            DeathInterval.SetActive(false);
            yield return new WaitForSeconds(2.0f);
        }

        this.gameObject.transform.position = startingSpawnPos;

        yield return null;
    }
}
