using UnityEngine;
using System.Collections;

public class CaptionLevelControl : MonoBehaviour {

    public static int visibilityTimer;

	// Use this for initialization
	
    void Start ()
    {
        visibilityTimer = 60;
	}
	
	// Update is called once per frame
	
    void Update ()
    {
        if (visibilityTimer == 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            visibilityTimer--;
        }
	}
}
