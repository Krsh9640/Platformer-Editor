using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public static int totalCoin = 0;

    private GameState gameState;

    private void Start()
    {
        gameState = GameObject.Find("Manager").GetComponent<GameState>();
    }

    private void Update()
    {
        if (gameState.isPlay == false)
        {
            totalCoin = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (gameState.isPlay == true)
        {
            if (other.tag == "Player")
            {
                totalCoin++;

                Destroy(gameObject);
            }
        }
    }
}
