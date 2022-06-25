using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Coin : MonoBehaviour
{
    public static int totalCoin = 0;

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player"){
            totalCoin++;

            Destroy(gameObject);
        }
    }


}
