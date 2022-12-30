using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public GameObject blink;
    public float speed;
    Rigidbody2D rb;

    private GameState gameState;

    private void Start()
    {
        blink = GameObject.Find("FlynnBlink");
        gameState = GameObject.Find("Manager").GetComponent<GameState>();
    }

    private void Update()
    {
        if (gameState.isPlay == true)
        {
            if (gameObject.name == "CannonBall(Clone")
            {
                CircleCollider2D collider2d = gameObject.GetComponent<CircleCollider2D>();
                collider2d.enabled = true;

                this.gameObject.tag = "Extras";
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(COSCannonBallDamage(other));
        }
    }

    public IEnumerator COSCannonBallDamage(Collider2D other)
    {
        PlayerMovement movement = other.GetComponent<PlayerMovement>();
        movement.currentHealth--;
        movement.SetInvincible(4);
        Invoke("EnableBlink", 0f);
        Invoke("DisableBlink", 0.1f);
        yield return new WaitForSeconds(4);
    }

    void OnBecameInvisible()
    {
        if (this.gameObject.name == "CannonBall(Clone)")
        {
            Destroy(this.gameObject);
        }
    }

    private void EnableBlink()
    {
        blink.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void DisableBlink()
    {
        blink.GetComponent<SpriteRenderer>().enabled = false;
    }
}
