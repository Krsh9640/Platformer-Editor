using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class vCamTrigger : MonoBehaviour
{
    [SerializeField] CinemachineBrain vCamBrain;
    public bool IsEnabled = true;
    private GameState gameState;

    private void Start()
    {
        gameState = GameObject.Find("Manager").GetComponent<GameState>();

        if (gameState.isPlay == false)
        {
            vCamBrain.GetComponent<CinemachineBrain>().enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameState.isPlay == true)
        {
            if (other.tag == "Player")
            {
                vCamBrain.GetComponent<CinemachineBrain>().enabled = IsEnabled;
            }
            IsEnabled = !IsEnabled;
        }
    }
}
