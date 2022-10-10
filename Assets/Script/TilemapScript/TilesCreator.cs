using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TilesCreator : Singleton<TilesCreator>
{
    [SerializeField] Tilemap previewMap, 
    defaultMap;
    PlayerInput playerInput;

    TileBase tileBase;
    TilesObject selectedTile;

    Camera _camera;

    Vector2 mousePos;
    Vector3Int currentGridPosition;
    Vector3Int lastGridPosition;

    Vector3Int holdStartPosition;

    bool pointerOverUI = false;
    bool holdActive;

    BoundsInt bounds;

    public GameObject mushroom, bee, frog;
    public GameObject coin, key, potion, boots, flag, burner, cannon, 
                      buzzsaw, spikeball, spring, unlockedDoor, lockedDoor;
    
    public List<Tilemap> tilemaps = new List<Tilemap>();

    private GameObject manager;
    private GameState gameState;

    GameObject instantiatedPrefab;

    int UILayer;

    protected override void Awake(){
        base.Awake();
        playerInput = new PlayerInput();
        _camera = Camera.main;
    }

    void Start() {
        List<Tilemap> maps = FindObjectsOfType<Tilemap>().ToList();

        manager = GameObject.Find("Manager");
        gameState = manager.GetComponent<GameState>();

        UILayer = LayerMask.NameToLayer("UI");

        maps.ForEach(map => {
            if(map.name != "previewMap"){
                if(map.name != "defaultMap"){
                    tilemaps.Add(map);
                } 
            }
        });    
    }

    public void ClearTiles(){
        tilemaps.ForEach(map => {
            map.ClearAllTiles();
        });

        GameObject[] prefabsObject = FindGameObjectWithinLayer(8);

        foreach(GameObject go in prefabsObject)
        {
            Destroy(go);
        }
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
 
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    private void OnEnable(){
        if(playerInput != null){
            playerInput.Enable();
            playerInput.Gameplay.MousePosition.performed += OnMouseMove;

            playerInput.Gameplay.MouseLeftClick.performed += OnLeftClick;
            playerInput.Gameplay.MouseLeftClick.started += OnLeftClick;
            playerInput.Gameplay.MouseLeftClick.canceled += OnLeftClick;

            playerInput.Gameplay.MouseRightClick.performed += OnRightClick;
        }
    }

    private void OnDisable(){
        if(playerInput != null){
            playerInput.Disable();
            playerInput.Gameplay.MousePosition.performed -= OnMouseMove;

            playerInput.Gameplay.MouseLeftClick.performed -= OnLeftClick;
            playerInput.Gameplay.MouseLeftClick.started -= OnLeftClick;
            playerInput.Gameplay.MouseLeftClick.canceled -= OnLeftClick;

            playerInput.Gameplay.MouseRightClick.performed -= OnRightClick;
        }
    }

    private TilesObject SelectedTile{
        set {
            selectedTile = value;
            tileBase = selectedTile != null ? selectedTile.TileBase : null;

            UpdatePreview();
        }
    }


    private Tilemap tilemap{
        get{
            if (selectedTile != null && selectedTile.Category != null && selectedTile.Category.Tilemap != null) {
                return selectedTile.Category.Tilemap;
            } 
            return defaultMap;
        }
    }
    
    private void Update(){
        if (gameState.isPlay == false){
            if (selectedTile != null){
                Vector3 pos = _camera.ScreenToWorldPoint (mousePos);
                Vector3Int gridPos = previewMap.WorldToCell (pos);

            if(gridPos != currentGridPosition){
                lastGridPosition = currentGridPosition;
                currentGridPosition = gridPos;

                UpdatePreview();

                if(holdActive){
                    HandleDrawing();
                }
            }
            }

            if(gameState.isPlay == true){
                selectedTile = null;
            }
        }
    }

    private void OnMouseMove(InputAction.CallbackContext ctx){
        mousePos = ctx.ReadValue<Vector2>();
    }

    private void OnLeftClick(InputAction.CallbackContext ctx){
        if(gameState.isPlay == false){
            if(selectedTile != null && !IsPointerOverUIElement()){
            if(ctx.phase == InputActionPhase.Started){
                 holdActive = true;
                if(ctx.interaction is TapInteraction) {
                   
                    holdStartPosition = currentGridPosition;
                }
                HandleDrawing();
            }else {
                if (ctx.interaction is SlowTapInteraction || ctx.interaction is TapInteraction && ctx.phase == InputActionPhase.Performed){
                    holdActive = false;
                    HandleDrawingRelease();
                }
            }
        }
        }
    }
    

    private void OnRightClick(InputAction.CallbackContext ctx){
        Vector3 pos = _camera.ScreenToWorldPoint (mousePos);
        Vector3Int gridPos = previewMap.WorldToCell (pos);

        if(gameState.isPlay == false){
            if(selectedTile != null){
                SelectedTile = null;
            } else {
                Eraser(gridPos);
            }
        }
    }

    public void Eraser(Vector3Int position){
        tilemaps.ForEach(map => {
            map.SetTile(position, null);
        });
    }

    public void TileSelected(TilesObject tile){
        SelectedTile = tile;
    }

    void UpdatePreview(){
        previewMap.SetTile(lastGridPosition, null);
        previewMap.SetTile(currentGridPosition, tileBase);
    }

    private void HandleDrawing(){
        if(selectedTile != null){
            switch (selectedTile.PlaceType){
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

     private void HandleDrawingRelease(){
        if(selectedTile != null && pointerOverUI){
            switch (selectedTile.PlaceType){
                case PlaceType.Rectangle:
                    DrawBounds(tilemap);
                    previewMap.ClearAllTiles();
                    break;
            }
        }
    }       
    
    private void RectangleRenderer(){
        
        previewMap.ClearAllTiles();

        bounds.xMin = currentGridPosition.x < holdStartPosition.x ? currentGridPosition.x : holdStartPosition.x;
        bounds.xMax = currentGridPosition.x > holdStartPosition.x ? currentGridPosition.x : holdStartPosition.x;
        bounds.yMin = currentGridPosition.y < holdStartPosition.y ? currentGridPosition.y : holdStartPosition.y;
        bounds.yMax = currentGridPosition.y > holdStartPosition.y ? currentGridPosition.y : holdStartPosition.y;

        DrawBounds(previewMap);
    }

    private void DrawBounds (Tilemap map) {
        for (int x = bounds.xMin; x <= bounds.xMax; x++){
            for (int y = bounds.yMin; y <= bounds.yMax; y++){
                DrawItem(map, new Vector3Int (x, y, 0), tileBase);
            }
        }
    }

    private void DrawItem(Tilemap map, Vector3Int position, TileBase tileBase)
    {
        if (selectedTile.GameObject != null)
        {
            instantiatedPrefab = Instantiate(selectedTile.GameObject, currentGridPosition, transform.rotation);

            if(selectedTile.GameObject.name == "LockedDoor" || selectedTile.GameObject.name == "UnlockedDoor")
            {
                instantiatedPrefab.transform.position = new Vector3(currentGridPosition.x + 0.5f, currentGridPosition.y + 0.74f, 0);
            } else
            {
                instantiatedPrefab.transform.position = new Vector3(currentGridPosition.x + 0.5f, currentGridPosition.y + 0.5f, 0);
            }

        } else
        {
            tilemap.SetTile(position, tileBase);
        }
    }

    public GameObject[] FindGameObjectWithinLayer(int layer)
    {
        GameObject[] goArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];

        List<GameObject> goList = new List<GameObject>();

        for (int i = 0; i < goArray.Length; i++)
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
}
