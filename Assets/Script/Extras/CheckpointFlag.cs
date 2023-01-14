using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointFlag : MonoBehaviour
{
    public bool isPassed;

    private GameObject player;
    public Sprite CheckpointSprite;
    private PlayerMovement playerMovement;
    private GameState gameState;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        gameState = GameObject.Find("Manager").GetComponent<GameState>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameState.isPlay == true)
        {
            if (other.tag == "Player")
            {
                isPassed = true;
                this.gameObject.GetComponent<SpriteRenderer>().sprite = CheckpointSprite;

                playerMovement.startingSpawnPos = this.gameObject.transform.position;
            }
        }
    }
}
