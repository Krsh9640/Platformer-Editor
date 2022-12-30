using UnityEngine;
using System.Collections;

public class ConveyorSwitchControl : MonoBehaviour {

    bool state;
    Animator animator;
    SpriteRenderer spriteRenderer;
    General general;

	// Use this for initialization

	void Start ()
    {
        general = GameObject.Find("MainCamera").GetComponent<General>();
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame

	void Update ()
    {
	
	}

    void OnMouseDown()
    {
        state = !state;

        if (state)
        {
            general.conveyorDirectionForward = true;
            animator.Play("CSwitchR");
        }
        else
        {
            general.conveyorDirectionForward = false;
            animator.Play("CSwitchL");
        }
    }
}