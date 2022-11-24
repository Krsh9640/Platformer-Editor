using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    private GameObject manager;
    private GameState gameState;

    private void Start()
    {
        manager = GameObject.Find("Manager");

        if (manager != null)
        {
            gameState = manager.GetComponent<GameState>();
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (gameState != null)
            {
                if (gameState.isPlay == false)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}