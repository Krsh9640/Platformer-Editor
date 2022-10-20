using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    public GameObject manager;
    private GameState gameState;

    private Vector3 ResetCamera;

    [SerializeField] private Camera cam;

    private float levelMinX, levelMaxX, levelMinY, levelMaxY;

    private Collider2D col;
    [SerializeField] private GameObject constraint;

    private Vector3 dragOrigin;

    private void Awake()
    {
        col = constraint.GetComponent<PolygonCollider2D>();

        levelMinX = col.transform.position.x - col.bounds.size.x / 2f;
        levelMaxX = col.transform.position.x + col.bounds.size.x / 2f;

        levelMinY = col.transform.position.y - col.bounds.size.y / 2f;
        levelMaxY = col.transform.position.y + col.bounds.size.y / 2f;
    }

    private void Start()
    {
        gameState = manager.GetComponent<GameState>();

        ResetCamera = Camera.main.transform.position;
    }

    private void LateUpdate()
    {
        PanCamera();

        if (gameState.isPlay == true)
        {
            cam.transform.position = ResetCamera;
        }
    }

    private void PanCamera()
    {
        if (Input.GetMouseButtonDown(2))

            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(2))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);

            cam.transform.position = ClampCamera(cam.transform.position + difference);
        }

        if (cam.transform.position.y <= 0)
        {
            cam.transform.position = new Vector3(cam.transform.position.x, 0, cam.transform.position.z);
        }
        if (cam.transform.position.x < 0)
        {
            cam.transform.position = new Vector3(0, cam.transform.position.y, cam.transform.position.z);
        }
    }

    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        float minX = levelMinX + camWidth;
        float maxX = levelMaxX - camWidth;
        float minY = levelMinY + camHeight;
        float maxY = levelMaxY - camHeight;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxX);

        return new Vector3(newX, newY, targetPosition.z);
    }
}