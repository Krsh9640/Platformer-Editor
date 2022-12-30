using UnityEngine;
using System.Collections;

public class ConveyorControl : MonoBehaviour {

    public bool conveyorDirectionFwd;
    Animator animator;
    General general;
    PlayerControl script;
    SpriteRenderer spriteRenderer;

	// Use this for initialization

	void Start ()
    {
        general = GameObject.Find("MainCamera").GetComponent<General>();
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        StartAnimation();
	}
	
	// Update is called once per frame

	void Update ()
    {
        if (conveyorDirectionFwd != general.conveyorDirectionForward)
        {
            conveyorDirectionFwd = general.conveyorDirectionForward;
            StartAnimation();
        }
	}

    void StartAnimation()
    {

        switch(gameObject.name)
        {
            case "ConveyorW":
                
                if (conveyorDirectionFwd)
                {
                    animator.Play("Forwards_W");
                }
                else
                {
                    animator.Play("Backwards_W");
                }
                break;

            case "ConveyorM":
                
                if (conveyorDirectionFwd)
                {
                    animator.Play("Forwards_M");
                }
                else
                {
                    animator.Play("Backwards_M");
                }
                break;

            case "ConveyorO":
                
                if (conveyorDirectionFwd)
                {
                    animator.Play("Forwards_O");
                }
                else
                {
                    animator.Play("Backwards_O");
                }
                break;
        }
    }
}