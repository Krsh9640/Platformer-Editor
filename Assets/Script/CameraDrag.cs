using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    public GameObject manager;
    private GameState gameState;

    private Vector3 Origin;
    private Vector3 Difference;
    private Vector3 ResetCamera;

    private bool drag = false;

    [SerializeField] private Camera cam;

    private float levelMinX, levelMaxX, levelMinY, levelMaxY;

    private void Start() {
        gameState = manager.GetComponent<GameState>();

        ResetCamera = Camera.main.transform.position;
    }
    
    private void LateUpdate() {
        if(gameState.isPlay == false){
            if(Input.GetMouseButton(2)){
                Difference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
                if(drag == false){
                    drag = true;
                    Origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
            } else {
                drag = false;
            }

            if(drag){
                Camera.main.transform.position = Origin - Difference;
            }
        }

        if(gameState.isPlay == true){
            Camera.main.transform.position = ResetCamera;
        }
    }
}

