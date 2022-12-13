using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class vCamTrigger : MonoBehaviour
{
    [SerializeField] CinemachineBrain vCamBrain;
    public static bool IsEnabled = true;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player"){
            vCamBrain.GetComponent<CinemachineBrain>().enabled = IsEnabled;
        }
        IsEnabled = !IsEnabled;
    }
}
