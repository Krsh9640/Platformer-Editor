using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public GameObject spawnPoint, cannonBall;
    public GameObject[] cannonBallPrefabs;
    float timeBetween;
    public float startTimeBetween;

    private GameState gameState;

    private void Start()
    {
        gameState = GameObject.Find("Manager").GetComponent<GameState>();

        timeBetween = startTimeBetween;
        timeBetween = 0;

        cannonBall = GameObject.Find("CannonBall");
        cannonBallPrefabs = GameObject.FindGameObjectsWithTag("Extras");
    }

    private void Update()
    {
        if (gameState.isPlay == true)
        {
            if (timeBetween <= 0)
            {
                GameObject instCannonBall = Instantiate(cannonBall, spawnPoint.transform.position, spawnPoint.transform.rotation);
                instCannonBall.transform.SetParent(this.gameObject.transform);
                instCannonBall.AddComponent<CannonBall>();
                instCannonBall.GetComponent<CircleCollider2D>().enabled = true;
                Rigidbody2D rb = instCannonBall.GetComponent<Rigidbody2D>();
                rb.velocity = this.gameObject.transform.localScale.x * transform.right * 7;

                timeBetween = startTimeBetween;
            }
            else
            {
                timeBetween -= Time.deltaTime;
            }
        }
    }
}
