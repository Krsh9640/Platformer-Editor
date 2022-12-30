using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class UIControl : MonoBehaviour {

    public void LockEdit()
    {
        Editor.editLock = true;
    }

    // Use this for initialization

    void Start()
	{

    }

    public void UnlockEdit()
    {
        Editor.editLock = false;
    }

    // Update is called once per frame

    void Update()
    {

    }		
}