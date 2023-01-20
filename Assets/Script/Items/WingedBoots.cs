using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingedBoots : MonoBehaviour
{
    public bool isPicked = false;
    public GameObject bootsIcon;
    private GameState gameState;

    void Start()
    {
        bootsIcon = GameObject.Find("BootsIcon");
        gameState = GameObject.Find("Manager").GetComponent<GameState>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (gameState.isPlay == true)
        {
            if (other.tag == "Player")
            {
                isPicked = true;
                StartCoroutine(COBootsFunc(other, 10));
            }
        }
    }

    public IEnumerator COBootsFunc(Collider2D player, float duration)
    {
        bootsIcon.GetComponent<SpriteRenderer>().enabled = true;
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        CharacterController2D controller2D = player.GetComponent<CharacterController2D>();
        movement.runSpeed += 20;
        controller2D.m_JumpForce += 30;

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;

        yield return new WaitForSeconds(duration);

        movement.runSpeed -= 20;
        controller2D.m_JumpForce -= 30;

        bootsIcon.GetComponent<SpriteRenderer>().enabled = false;

        Destroy(gameObject);
    }
}
