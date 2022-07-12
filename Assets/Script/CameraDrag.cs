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
    [SerializeField] public Collider2D levelCollider;

    [SerializeField] private Camera cam;

    private float levelMinX, levelMaxX, levelMinY, levelMaxY;

    private void Start() {
        gameState = manager.GetComponent<GameState>();

        ResetCamera = Camera.main.transform.position;

        levelMinX = levelCollider.transform.position.x - levelCollider.bounds.size.x / 2f;
        levelMaxX = levelCollider.transform.position.x + levelCollider.bounds.size.x / 2f;

        levelMinY = levelCollider.transform.position.y - levelCollider.bounds.size.y / 2f;
        levelMaxY = levelCollider.transform.position.y + levelCollider.bounds.size.y / 2f;
    }
    
    private void LateUpdate() {
        if(gameState.isPlay == false){
            if(Input.GetMouseButton(2)){
                Difference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
                if(drag == false){
                    drag = true;
                    Origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    cam.transform.position = ClampCamera(cam.transform.position + Difference);
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

    private Vector3 ClampCamera(Vector3 targetPosition){
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        float minX = levelMinX + camWidth;
        float maxX = levelMaxX - camWidth;
        float minY = levelMinY + camHeight;
        float maxY = levelMaxY - camHeight;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, targetPosition.z);
    }
}

