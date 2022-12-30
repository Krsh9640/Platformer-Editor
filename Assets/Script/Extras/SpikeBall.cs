using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBall : MonoBehaviour
{
    public GameObject blink;
    public bool isTouching = false;

    private void Update() {
        blink = GameObject.Find("FlynnBlink");
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Player"){
            isTouching = true;
            StartCoroutine(COBallSpike(other));
            isTouching = !isTouching;
        }
    }

    public IEnumerator COBallSpike(Collision2D other){
        PlayerMovement movement = other.gameObject.GetComponent<PlayerMovement>();
        while(isTouching == true){
            movement.currentHealth--;
            movement.SetInvincible(3);
            Invoke("EnableBlink", 0f);
            Invoke("DisableBlink", 0.1f);            
            isTouching = !isTouching;
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
