using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnlockedDoor : MonoBehaviour
{
    public GameObject manager, winText, player;
    public GameState gameState;
    public PlayerMovement movement;
    public Animator playerAnimator;

    public GameObject particle1, particle2;

    public bool isWin = false;

    private void Update() {
        manager = GameObject.Find("Manager");
        gameState = manager.GetComponent<GameState>();
        winText = GameObject.Find("WinText");
        player = GameObject.FindWithTag("Player"); 
        movement = player.GetComponent<PlayerMovement>();
        playerAnimator = player.GetComponent<Animator>();

        particle1 = GameObject.Find("ParticleSystem1");
        particle2 = GameObject.Find("ParticleSystem2");

    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player"){
            if(gameState.isPlay == true){
                StartCoroutine(COWinText());
            } 
        }
    }

    public IEnumerator COWinText(){
        isWin = true;
        
        winText.GetComponent<TMP_Text>().enabled = true;

        particle1.GetComponent<ParticleSystem>().Play();
        particle2.GetComponent<ParticleSystem>().Play();

        playerAnimator.SetFloat("Speed", 0);

        movement.enabled = false;
        gameState.GetComponentEnemy(false);

        yield return new WaitForSeconds(5);

        gameState.StopPlaying();
    }
}
