using UnityEngine;
using System.Collections;

public class SwitchControl : MonoBehaviour {

    bool state;
    Sprite[] sprite;
    SpriteRenderer spriteRenderer;

	// Use this for initialization

	void Start ()
    {
        General general = GameObject.Find("MainCamera").GetComponent<General>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        sprite = new Sprite[2];
        sprite[0] = general.sprite[0];
        sprite[1] = general.sprite[1];

        //Status zum Tag setzen

        if (gameObject.tag == "Wall")
        {
            state = true;
        }
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
            spriteRenderer.sprite = sprite[1];
            gameObject.tag = "Wall";
        }
        else
        {
            spriteRenderer.sprite = sprite[0];
            gameObject.tag = "Untagged";
        }
    }
}
