using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    private GameObject manager;
    private GameState gameState;

    private void Start()
    {
        manager = GameObject.Find("Manager");
        gameState = manager.GetComponent<GameState>();
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if(gameState.isPlay == false)
            {
                Destroy(gameObject);
            }
        }
    }
}
