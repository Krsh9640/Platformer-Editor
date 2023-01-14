using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvinciblePotion : MonoBehaviour
{
    public bool isPicked = false;

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player"){
            isPicked = true;
            StartCoroutine(COPotionFunc(other, 10));
        }
    }

    public IEnumerator COPotionFunc(Collider2D player, float duration){
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        movement.currentHealth = 5;
        
        yield return new WaitForSeconds(duration);

        Destroy(gameObject);
    }
}
