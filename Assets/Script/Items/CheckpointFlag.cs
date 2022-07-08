using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointFlag : MonoBehaviour
{
    public bool isPassed;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player"){
            isPassed = true;
        }
    }
}
