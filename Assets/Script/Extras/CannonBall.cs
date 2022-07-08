using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public GameObject blink;
    public float speed;
    Rigidbody2D rb;

    private void Update() {
        blink = GameObject.Find("FlynnBlink");

        if(this.gameObject.name == "CannonBall(Clone)"){
            this.gameObject.tag = "Extras";
            rb = GetComponent<Rigidbody2D>();
            rb.velocity = transform.right * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player"){
            StartCoroutine(COSCannonBallDamage(other));
        }
    }

    public IEnumerator COSCannonBallDamage(Collider2D other){
        PlayerMovement movement = other.GetComponent<PlayerMovement>();
            movement.currentHealth--;
            movement.SetInvincible(4);
            Invoke("EnableBlink", 0f);
            Invoke("DisableBlink", 0.1f);
            yield return new WaitForSeconds(4);
    }

    void OnBecameInvisible() {
        if(this.gameObject.name == "CannonBall(Clone)"){
            Destroy(this.gameObject);
        }   
    }

    private void EnableBlink(){
        blink.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void DisableBlink(){
        blink.GetComponent<SpriteRenderer>().enabled = false;
    }
}
