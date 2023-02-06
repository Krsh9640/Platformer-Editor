using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStomp : MonoBehaviour
{
    private int damageToDeal = 1;

    private Rigidbody2D rb;

    private float bounceForce = 5f;

    private GameState gameState;

    void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody2D>();
        gameState = GameObject.Find("Manager").GetComponent<GameState>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement movement = transform.parent.GetComponent<PlayerMovement>();

        if (gameState.isPlay == true)
        {
            if (other.name == "Frog" || other.name == "Bee" || other.name == "Mushroom")
            {
                other.GetComponent<EnemyHP>().TakeDamage(damageToDeal);
                rb.AddForce(transform.up * bounceForce, ForceMode2D.Force);
            }
            else if (other.name == "CannonBall")
            {
                rb.AddForce(transform.up * bounceForce, ForceMode2D.Force);
            }
        }
    }
}
