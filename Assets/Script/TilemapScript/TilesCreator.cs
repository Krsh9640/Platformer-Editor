using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Tilemaps;

public class TilesCreator : Singleton<TilesCreator>
{
    [SerializeField]
    private Tilemap previewMap,
    defaultMap;

    private PlayerInput playerInput;

    private TileBase tileBase;
    public TilesObject selectedTile;

    private Camera _camera;

    private Vector2 mousePos;
    private Vector3Int currentGridPosition;
    private Vector3Int lastGridPosition;

    private Vector3Int holdStartPosition;

    private bool pointerOverUI = false;
    private bool holdActive;

    private BoundsInt bounds;

    public GameObject mushroom, bee, frog;

    public GameObject coin, key, potion, boots, flag, burner, cannon,
                      buzzsaw, spikeball, spring, unlockedDoor, lockedDoor;

    public List<Tilemap> tilemaps = new List<Tilemap>();

    private GameObject manager;
    private GameState gameState;

    private int UILayer;

    public AudioClip tilePlace, tileDelete;
    public AudioSource audioSource;

    public GameObject[] prefabObject;
    [SerializeField] private GameObject selectedTileGameobject, instantiatedTileGameObject;
    public bool isSelected = false, isFlipped = false;

    [SerializeField] private Vector3 originalXScale, newXScale;

    protected override void Awake()
    {
        base.Awake();
        playerInput = new PlayerInput();
        _camera = Camera.main;
    }

    private void Start()
    {
        List<Tilemap> maps = FindObjectsOfType<Tilemap>().ToList();

        manager = GameObject.Find("Manager");
        gameState = manager.GetComponent<GameState>();

        UILayer = LayerMask.NameToLayer("UI");

        maps.ForEach(map =>
        {
            if (map.name != "previewMap")
            {
                if (map.name != "defaultMap")
                {
                    tilemaps.Add(map);
                }
            }
        });
    }

    public void ClearTiles()
    {
        tilemaps.ForEach(map =>
        {
            map.ClearAllTiles();
        });
    }

    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }

    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }

    private static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    private void OnEnable()
    {
        if (playerInput != null)
        {
            playerInput.Enable();
            playerInput.Gameplay.MousePosition.performed += OnMouseMove;

            playerInput.Gameplay.MouseLeftClick.performed += OnLeftClick;
            playerInput.Gameplay.MouseLeftClick.started += OnLeftClick;
            playerInput.Gameplay.MouseLeftClick.canceled += OnLeftClick;

            playerInput.Gameplay.MouseRightClick.performed += OnRightClick;
        }
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.Disable();
            playerInput.Gameplay.MousePosition.performed -= OnMouseMove;

            playerInput.Gameplay.MouseLeftClick.performed -= OnLeftClick;
            playerInput.Gameplay.MouseLeftClick.started -= OnLeftClick;
            playerInput.Gameplay.MouseLeftClick.canceled -= OnLeftClick;

            playerInput.Gameplay.MouseRightClick.performed -= OnRightClick;
        }
    }

    public TilesObject SelectedTile
    {
        set
        {
            selectedTile = value;
            tileBase = selectedTile != null ? selectedTile.Tile : null;

            UpdatePreview();
        }
    }

    private Tilemap tilemap
    {
        get
        {
            if (selectedTile != null && selectedTile.Category != null && selectedTile.Category.Tilemap != null)
            {
                return selectedTile.Category.Tilemap;
            }
            return defaultMap;
        }
    }

    private void Update()
    {
        if (gameState.isPlay == false)
        {
            prefabObject = FindGameObjectsWithLayer(8);

            if (prefabObject != null)
            {
                foreach (GameObject go in prefabObject)
                {
                    if (go.transform.parent != previewMap.transform)
                    {
                        go.tag = "hasPlaced";
                    }
                }
            }

            if (selectedTile != null)
            {
                Vector3 pos = _camera.ScreenToWorldPoint(mousePos);
                Vector3Int gridPos = previewMap.WorldToCell(pos);

                if (gridPos != currentGridPosition)
                {
                    lastGridPosition = currentGridPosition;
                    currentGridPosition = gridPos;

                    UpdatePreview();

                    if (previewMap.transform.childCount > 0 && previewMap.transform.GetChild(0).gameObject != null)
                    {
                        selectedTileGameobject = previewMap.transform.GetChild(0).gameObject;

                        originalXScale = selectedTileGameobject.gameObject.transform.localScale;

                        if (selectedTileGameobject != null && isSelected == true)
                        {
                            if (selectedTileGameobject.tag != "hasPlaced" && isFlipped == false)
                            {
                                newXScale = new Vector3(-selectedTileGameobject.transform.localScale.x,
                                selectedTileGameobject.transform.localScale.y, 0);

                                selectedTileGameobject.transform.localScale = newXScale;
                            }
                            else if (selectedTileGameobject.tag != "hasPlaced" && isFlipped == true)
                            {
                                newXScale = originalXScale;
                                selectedTileGameobject.transform.localScale = newXScale;
                            }
                        }
                    }

                    if (holdActive)
                    {
                        HandleDrawing();
                    }
                }
            }
        }
        if (gameState.isPlay == true)
        {
            selectedTile = null;
            SelectedTile = null;
        }
    }

    public GameObject[] FindGameObjectsWithLayer(int layer)
    {
        GameObject[] goArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        List<GameObject> goList = new List<GameObject>();
        for (var i = 0; i < goArray.Length; i++)
        {
            if (goArray[i].layer == layer)
            {
                goList.Add(goArray[i]);
            }
        }
        if (goList.Count == 0)
        {
            return null;
        }
        return goList.ToArray();
    }

    private void OnMouseMove(InputAction.CallbackContext ctx)
    {
        mousePos = ctx.ReadValue<Vector2>();
    }

    private void OnLeftClick(InputAction.CallbackContext ctx)
    {
        if (gameState.isPlay == false)
        {
            if (selectedTile != null && !IsPointerOverUIElement())
            {
                if (ctx.phase == InputActionPhase.Started)
                {
                    holdActive = true;
                    if (ctx.interaction is TapInteraction)
                    {
                        holdStartPosition = currentGridPosition;
                    }
                    HandleDrawing();
                }
                else
                {
                    if (ctx.interaction is SlowTapInteraction || ctx.interaction is TapInteraction && ctx.phase == InputActionPhase.Performed)
                    {
                        holdActive = false;
                        HandleDrawingRelease();
                    }
                }
            }
        }
    }

    private void OnRightClick(InputAction.CallbackContext ctx)
    {
        Vector3 pos = _camera.ScreenToWorldPoint(mousePos);
        Vector3Int gridPos = previewMap.WorldToCell(pos);

        if (gameState.isPlay == false)
        {
            if (selectedTile != null)
            {
                SelectedTile = null;
                isSelected = false;
                isFlipped = false;
            }
            else if (selectedTile == null)
            {
                Eraser(gridPos);
            }
        }
    }

    public void FlipTile()
    {
        if (isSelected == false)
        {
            isSelected = true;
        }
    }

    public void TileSelected(TilesObject tile)
    {
        SelectedTile = tile;
    }

    public void Eraser(Vector3Int position)
    {
        tilemaps.ForEach(map =>
        {
            map.SetTile(position, null);
        });
    }

    private void UpdatePreview()
    {
        previewMap.SetTile(lastGridPosition, null);
        previewMap.SetTile(currentGridPosition, tileBase);
    }

    private void HandleDrawing()
    {
        if (selectedTile != null)
        {
            switch (selectedTile.PlaceType)
            {
                case PlaceType.Single:
                default:
                    DrawItem(tilemap, currentGridPosition, tileBase);
                    break;

                case PlaceType.Rectangle:
                    RectangleRenderer();
                    break;
            }
        }
    }

    private void HandleDrawingRelease()
    {
        if (selectedTile != null && pointerOverUI)
        {
            switch (selectedTile.PlaceType)
            {
                case PlaceType.Rectangle:
                    DrawBounds(tilemap);
                    previewMap.ClearAllTiles();
                    break;
            }
        }
    }

    private void RectangleRenderer()
    {
        previewMap.ClearAllTiles();

        bounds.xMin = currentGridPosition.x < holdStartPosition.x ? currentGridPosition.x : holdStartPosition.x;
        bounds.xMax = currentGridPosition.x > holdStartPosition.x ? currentGridPosition.x : holdStartPosition.x;
        bounds.yMin = currentGridPosition.y < holdStartPosition.y ? currentGridPosition.y : holdStartPosition.y;
        bounds.yMax = currentGridPosition.y > holdStartPosition.y ? currentGridPosition.y : holdStartPosition.y;

        DrawBounds(previewMap);
    }

    private void DrawBounds(Tilemap map)
    {
        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y <= bounds.yMax; y++)
            {
                DrawItem(map, new Vector3Int(x, y, 0), tileBase);
            }
        }
    }

    private void DrawItem(Tilemap map, Vector3Int position, TileBase tileBase)
    {
        tilemap.SetTile(position, tileBase);

        instantiatedTileGameObject = map.GetInstantiatedObject(position);
        Debug.Log(instantiatedTileGameObject);

        if (instantiatedTileGameObject != null)
        {
            instantiatedTileGameObject.name = tileBase.name;
            instantiatedTileGameObject.transform.localScale = selectedTileGameobject.transform.localScale;
        }
        audioSource.PlayOneShot(tilePlace, 0.5f);
    }
}