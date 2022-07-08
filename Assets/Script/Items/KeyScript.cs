using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour
{
    public bool isPicked = false;
    public GameObject keyIcon;

    void Start() {
        keyIcon = GameObject.Find("KeyIcon");
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player"){
            isPicked = true;
            keyIcon.GetComponent<SpriteRenderer>().enabled = true;
            StartCoroutine(COKeyFunc());
        }
    }

    public IEnumerator COKeyFunc(){

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;

        yield return new WaitForSeconds(4);

        Destroy(gameObject);
    }
}
