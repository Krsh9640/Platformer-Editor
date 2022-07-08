using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public GameObject spawnPoint, cannonBall, gameState;
    public GameObject [] cannonBallPrefabs;
    float timeBetween;
    public float startTimeBetween;

    private void Start() {
        gameState = GameObject.Find("Manager");

        timeBetween = startTimeBetween;
        timeBetween = 0;
    }

    private void Update() {
        spawnPoint = GameObject.Find("CannonBallSpawnPoint");
        cannonBall = GameObject.Find("CannonBall");
        cannonBallPrefabs = GameObject.FindGameObjectsWithTag("Extras");

        if(gameState.TryGetComponent(out GameState gameStateScript)){
            if(gameStateScript.isPlay == true){
                if(timeBetween <= 0){
                    Instantiate(cannonBall, spawnPoint.transform.position, spawnPoint.transform.rotation);

                    foreach(GameObject i in cannonBallPrefabs){
                        i.AddComponent<CannonBall>();
                        if(i.TryGetComponent(out CircleCollider2D collider)){
                            collider.enabled = true;
                        } else {
                            i.AddComponent<CircleCollider2D>();
                        }
                    }
                    timeBetween = startTimeBetween;
                } else {
                    timeBetween -= Time.deltaTime;
                }   
            }
        } 
    }
}
