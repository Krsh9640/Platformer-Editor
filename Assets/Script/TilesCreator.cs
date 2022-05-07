using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.EventSystems;

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

    bool pointerOverUI = false;
    bool holdActive;
    Vector3Int holdStartPosition;

    BoundsInt bounds;

    protected override void Awake(){
        base.Awake();
        playerInput = new PlayerInput();
        _camera = Camera.main;
    }

    private void OnEnable(){
        playerInput.Enable();

        playerInput.Gameplay.MousePosition.performed += OnMouseMove;

        playerInput.Gameplay.MouseLeftClick.performed += OnLeftClick;
        playerInput.Gameplay.MouseLeftClick.started += OnLeftClick;
        playerInput.Gameplay.MouseLeftClick.canceled += OnLeftClick;

        playerInput.Gameplay.MouseRightClick.performed += OnRightClick;
    }

    private void OnDisable(){
        playerInput.Disable();

        playerInput.Gameplay.MousePosition.performed -= OnMouseMove;

        playerInput.Gameplay.MouseLeftClick.performed -= OnLeftClick;
        playerInput.Gameplay.MouseLeftClick.started -= OnLeftClick;
        playerInput.Gameplay.MouseLeftClick.canceled -= OnLeftClick;

        playerInput.Gameplay.MouseRightClick.performed -= OnRightClick;
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
        pointerOverUI = !EventSystem.current.IsPointerOverGameObject(-1);

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

    private void OnMouseMove(InputAction.CallbackContext ctx){
        mousePos = ctx.ReadValue<Vector2>();
    }

    private void OnLeftClick(InputAction.CallbackContext ctx){
        if(selectedTile != null && pointerOverUI){
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
        SelectedTile = null;
    }

    public void TileSelected(TilesObject tile){
        SelectedTile = tile;
    }

    private void UpdatePreview(){
        previewMap.SetTile (lastGridPosition, null);
        previewMap.SetTile(currentGridPosition, tileBase);
    }

    private void HandleDrawing(){
        if(selectedTile != null && pointerOverUI){
            switch (selectedTile.PlaceType){
                case PlaceType.Single:
                default: 
                    DrawItem();
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
                map.SetTile(new Vector3Int (x, y, 0), tileBase);
            }
        }
    }

    private void DrawItem(){
        tilemap.SetTile (currentGridPosition, tileBase);
    }
}
