using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burner : MonoBehaviour
{
    public GameObject blink;
    public bool isTouching = false;
    public Animator animator;

    public GameObject gameState;

    private void Update() {
        blink = GameObject.Find("FlynnBlink");
        animator = this.gameObject.GetComponent<Animator>();

        gameState = GameObject.Find("Manager");
        if(gameState.TryGetComponent(out GameState gameStateScript)){
            if(gameStateScript.isPlay == false){
                animator.enabled = false;
            } else {
                animator.enabled = true;
                StartCoroutine(shootFire());
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player"){
            isTouching = true;
            StartCoroutine(COSBurnerDamage(other));
            isTouching = !isTouching;
        }
    }

    public IEnumerator COSBurnerDamage(Collider2D other){
        PlayerMovement movement = other.GetComponent<PlayerMovement>();
        while(isTouching == true){
            movement.currentHealth--;
            movement.SetInvincible(4);
            Invoke("EnableBlink", 0f);
            Invoke("DisableBlink", 0.1f);
            yield return new WaitForSeconds(4);
            isTouching = !isTouching;
        }
    }

    public IEnumerator shootFire(){
        while(true){
            animator.SetBool("isShooting", true);
            yield return new WaitForSeconds(3);
        }
    }

    private void EnableBlink(){
        blink.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void DisableBlink(){
        blink.GetComponent<SpriteRenderer>().enabled = false;
    }
}
