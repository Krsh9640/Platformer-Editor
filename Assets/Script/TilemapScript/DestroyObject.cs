using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    private GameState gameState;

    private void Start()
    {
        gameState = GameObject.Find("Manager").GetComponent<GameState>();
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