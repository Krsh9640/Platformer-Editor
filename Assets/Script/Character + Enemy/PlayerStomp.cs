using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStomp : MonoBehaviour
{
    private int damageToDeal = 1;

    private Rigidbody2D rb;

    private float bounceForce = 12f;

    void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        PlayerMovement movement = transform.parent.GetComponent<PlayerMovement>();

        if(other.tag == "Enemy"){
            other.GetComponent<EnemyHP>().TakeDamage(damageToDeal);
            rb.AddForce(transform.up * bounceForce, ForceMode2D.Impulse);
        } else if(other.tag == "Extras" && movement.isGrounded == false){
            bounceForce = 20f;
            rb.AddForce(transform.up * bounceForce, ForceMode2D.Impulse);
        }
    }
}
