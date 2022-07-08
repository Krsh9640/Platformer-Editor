using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;

    float horizontalMove = 0f;
    public float runSpeed = 100f;

    public bool jump = false;
    public bool isGrounded = true;

    public int playerHealth = 3;
    public int currentHealth;

    bool isInvincible = false;

    public GameObject blink;
    public TMP_Text healthText;

    void Awake() {
        currentHealth = playerHealth;
    }

    void Update()
    {
        if(healthText.text != currentHealth.ToString()){
            healthText.text = currentHealth.ToString();
        }

        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        
        if (Input.GetButtonDown("Jump") && isGrounded == true){
            jump = true;
            isGrounded = false;
            animator.SetBool("isJumping", true);                
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
    
    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.collider.gameObject.layer == LayerMask.NameToLayer("Platform")){
            isGrounded = true;
        }   
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Enemy" && isInvincible == false && isGrounded == true){
            currentHealth -= 1;
            SetInvincible(3);
            Invoke("EnableBlink", 0f);
            Invoke("DisableBlink", 0.1f);
        }
    }

    public void SetInvincible(float invincibleTime){
        isInvincible = true;

        StartCoroutine(COSetInvincible(invincibleTime));
    }

    void SetDamageable(){
        isInvincible = false;
    }

    public IEnumerator COSetInvincible (float invincibleTime){
        yield return new WaitForSeconds(invincibleTime);
        SetDamageable();
    }

    private void EnableBlink(){
        blink.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void DisableBlink(){
        blink.GetComponent<SpriteRenderer>().enabled = false;
    }
}
