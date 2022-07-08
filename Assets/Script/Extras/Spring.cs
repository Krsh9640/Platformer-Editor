using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    private float bounceForce = 15f;
    public Animator animator;
    public GameObject manager;

    public bool isJumped = false;

    private float timeElapsed;
    public GameObject player;

    private void Update() {
        animator = this.gameObject.GetComponent<Animator>();

        manager= GameObject.Find("Manager");

        timeElapsed += Time.deltaTime;
    }
    
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.transform.CompareTag("Player")){
            if(isJumped == false){
                animator.SetBool("isJumping", true);

                Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
                rb.AddForce(transform.up * bounceForce, ForceMode2D.Impulse);

                isJumped = !isJumped;
            } else {
                animator.SetBool("isJumping", false);

                Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
                rb.AddForce(transform.up * bounceForce, ForceMode2D.Impulse);

                animator.SetBool("isJumping", true);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if(other.transform.CompareTag("Player")){
            if(timeElapsed > 1 || isJumped == true){
                animator.SetBool("isJumping", false);
                
                timeElapsed = 0;    
                isJumped = !isJumped;
            }
        }
    }
}