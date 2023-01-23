using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvinciblePotion : MonoBehaviour
{
    public bool isPicked = false;

    private GameState gameState;

    private void Start()
    {
        gameState = GameObject.Find("Manager").GetComponent<GameState>();
    }

    private void Update()
    {
        if (gameState.isPlay == false)
        {
            this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            this.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (gameState.isPlay == true)
        {
            if (other.tag == "Player")
            {
                isPicked = true;
                StartCoroutine(COPotionFunc(other, 10));
            }
        }
    }

    public IEnumerator COPotionFunc(Collider2D player, float duration)
    {
        this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        movement.currentHealth = 5;

        yield return new WaitForSeconds(duration);

        Destroy(gameObject);
    }
}
