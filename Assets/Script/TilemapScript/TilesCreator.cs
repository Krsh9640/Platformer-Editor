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
    public GameObject coin, key, potion, boots, flag, burner, 
                      cannon, buzzsaw, spikeball, spring, unlockedDoor;
    
    public List<Tilemap> tilemaps = new List<Tilemap>();

    protected override void Awake(){
        base.Awake();
        playerInput = new PlayerInput();
        _camera = Camera.main;
    }

    void Start() {
        List<Tilemap> maps = FindObjectsOfType<Tilemap>().ToList();

        maps.ForEach(map => {
            if(map.name != "previewMap"){
                if(map.name != "defaultMap"){
                    tilemaps.Add(map);
                } 
            }
        });    
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
            if (selectedTile != null && selectedTile.Category != null && selectedTile.Category.Tilemap != null){
                return selectedTile.Category.Tilemap;
            }
        
            return defaultMap;
        }
    }
    
    private void Update(){
        

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
    }

    private bool isMouseOverUI(){
        return EventSystem.current.IsPointerOverGameObject();
    }    

    private void OnMouseMove(InputAction.CallbackContext ctx){
        mousePos = ctx.ReadValue<Vector2>();
    }

    private void OnLeftClick(InputAction.CallbackContext ctx){
        if(selectedTile != null && !isMouseOverUI()){
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
    

    private void OnRightClick(InputAction.CallbackContext ctx){
        Vector3 pos = _camera.ScreenToWorldPoint (mousePos);
        Vector3Int gridPos = previewMap.WorldToCell (pos);

        if(selectedTile != null){
            SelectedTile = null;    
        } else {
            Eraser(gridPos);
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

    private void DrawItem(Tilemap map, Vector3Int position, TileBase tileBase){
        if(tileBase.name == "MushroomIdle1"){
            GameObject instMush = Instantiate(mushroom, currentGridPosition, transform.rotation);
            instMush.transform.position = new Vector3(currentGridPosition.x + 0.5f, currentGridPosition.y + 0.5f, 0);
        } else if(tileBase.name == "BeeIdle1"){        
            GameObject instBee = Instantiate(bee,  currentGridPosition, transform.rotation); 
            instBee.transform.position = new Vector3(currentGridPosition.x + 0.5f, currentGridPosition.y + 0.5f, 0);
        } else if(tileBase.name == "FrogIdle1"){
            GameObject instFrog = Instantiate(frog, currentGridPosition, transform.rotation);
            instFrog.transform.position = new Vector3(currentGridPosition.x + 0.5f, currentGridPosition.y + 0.5f, 0);
        } else if(tileBase.name == "Key"){
            GameObject instKey = Instantiate(key, currentGridPosition, transform.rotation);
            instKey.transform.position = new Vector3(currentGridPosition.x + 0.5f, currentGridPosition.y + 0.5f, 0);
        } else if(tileBase.name == "Coin") {
            GameObject instCoin = Instantiate(coin, currentGridPosition, transform.rotation);
            instCoin.transform.position = new Vector3(currentGridPosition.x + 0.5f, currentGridPosition.y + 0.5f, 0);
        } else if(tileBase.name == "WingedShoes"){
            GameObject instBoots = Instantiate(boots, currentGridPosition, transform.rotation);
            instBoots.transform.position = new Vector3(currentGridPosition.x + 0.5f, currentGridPosition.y + 0.5f, 0);
        } else if(tileBase.name == "InvinciblePotion"){
            GameObject instPotion = Instantiate(potion, currentGridPosition, transform.rotation);
            instPotion.transform.position = new Vector3(currentGridPosition.x + 0.5f, currentGridPosition.y + 0.5f, 0);
        } else if(tileBase.name == "CheckpointFlag1"){
            GameObject instFlag = Instantiate(flag, currentGridPosition, transform.rotation);
            instFlag.transform.position = new Vector3(currentGridPosition.x + 0.5f, currentGridPosition.y + 0.5f, 0);
        }else if(tileBase.name == "Burner"){
            GameObject instBurner = Instantiate(burner, currentGridPosition, transform.rotation);
            instBurner.transform.position = new Vector3(currentGridPosition.x + 0.5f, currentGridPosition.y + 0.5f, 0);
        } else if(tileBase.name == "Cannon"){
            GameObject instCannon = Instantiate(cannon, currentGridPosition, transform.rotation);
            instCannon.transform.position = new Vector3(currentGridPosition.x + 0.5f, currentGridPosition.y + 0.5f, 0);
        } else if(tileBase.name == "Buzzsaw"){
            GameObject instBuzzsaw = Instantiate(buzzsaw, currentGridPosition, transform.rotation);
            instBuzzsaw.transform.position = new Vector3(currentGridPosition.x + 0.5f, currentGridPosition.y + 0.5f, 0);
        } else if(tileBase.name == "Spikeball"){
            GameObject instSpikeball = Instantiate(spikeball, currentGridPosition, transform.rotation);
            instSpikeball.transform.position = new Vector3(currentGridPosition.x + 0.5f, currentGridPosition.y + 0.5f, 0);
        } else if(tileBase.name == "Spring2"){
            GameObject instSpring = Instantiate(spring, currentGridPosition, transform.rotation);
            instSpring.transform.position = new Vector3(currentGridPosition.x + 0.5f, currentGridPosition.y - 0.024f, 0);
        } else if(tileBase.name == "UnlockedDoor1"){
            GameObject instUnlokcedDoor = Instantiate(unlockedDoor, currentGridPosition, transform.rotation);
            instUnlokcedDoor.transform.position = new Vector3(currentGridPosition.x + 0.5f, currentGridPosition.y + 0.75f, 0);
        }
        else {
            tilemap.SetTile (position, tileBase);
        }
    }
}
