using UnityEngine;
using System.Collections;

public class General : MonoBehaviour {

    public static int programState = 0;                                // 0 = Editor; 1 = Preview Playing; 2 = Playing
    public static float scrollFadingAmount = 1.05f;
    public bool conveyorDirectionForward;
    public float timer;
    public Sprite[] sprite;
    bool pointerState;
    float screenWorldRatio;
    Camera camera;
    Vector3 pointerScreenPosition, pointerOffset;

    void Assignments()
    {
        camera = gameObject.GetComponent<Camera>();
    }

	// Use this for initialization

	void Start ()
    {
        Assignments();
    }

	// Update is called once per frame

	void Update ()
    {
        
    }   
}