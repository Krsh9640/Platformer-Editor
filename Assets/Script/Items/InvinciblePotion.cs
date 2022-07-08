using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvinciblePotion : MonoBehaviour
{
    public bool isPicked = false;
    public GameObject potionIcon;

    void Start() {
        potionIcon = GameObject.Find("PotionIcon"); 
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player"){
            isPicked = true;
            StartCoroutine(COPotionFunc(other, 10));
        }
    }

    public IEnumerator COPotionFunc(Collider2D player, float duration){
        potionIcon.GetComponent<SpriteRenderer>().enabled = true;
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        movement.currentHealth = 5;
        
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;

        yield return new WaitForSeconds(duration);

        potionIcon.GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject);
    }
}
