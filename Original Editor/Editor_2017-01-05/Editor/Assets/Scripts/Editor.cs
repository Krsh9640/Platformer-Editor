using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Editor : MonoBehaviour {

    struct sitem
    {
        public int[,] tileID;
        public int layer;
        public int x;
        public int y;
        public int xOffset;
        public int yOffset;
    }

    public struct smatrix
	{
        public GameObject tileContainer;
		public int spriteID;
    }

    struct smatrixIndex
    {
        public bool reset;
        public int x;
        public int y;
        public int xTarget;
        public int yTarget;
    }

    public static bool drawMode;
    public static bool editLock;
    public static bool gridVisibility = true;
    public static bool moveMode;
    public static bool currentItemVariation;
    public static int currentItemID, variationItemID;
    public static int editMode;
    public static int matrixWidth = 4 * 16, matrixHeight = 3 * 9;
    public static int[] spriteIDs;
    public static int tileSize = 64;
    public static GameObject grid;
    public static Sprite[] sprite;
    public static smatrix[,,] matrix;
    public static Vector2 matrixOffset;
    public int matrixViewWidth = 16, matrixViewHeight = 9;
    const int buttonAdd = 0;
    const int buttonEraser = 2;
    const int buttonPan = 1;
    const int buttonPlay = 6;
    const int idButtonPlay = 9;
    const int idButtonStop = 13;
    bool itemEditingAllowed;
    bool itemDragged;
    bool playState;
    bool pointerReleased, pointerState;
    bool scrollFading;
    float cameraSize = 288, cameraSizeDefault = 288;
    float displayRatio = 1920f / 1080f;
    float screenWorldRatio;
    float scrollFadingAmount;
    int buttonsCount, spritesCount;
    int currentLevel = 1;
    int editModePrevious;
    int gridWidth = 3 * 16 * 64, gridHeight = 3 * 9 * 64;
    int zoomLevel;
    string[] buttonName, spriteList;
    Button[] button;
    Camera camera;
    Game game;
    GameObject captionLevel;
    GameObject player;
    GameObject tileContainer;
    GameObject menuBar;
    SpriteState buttonSprites;
    sitem item;
    smatrixIndex matrixIndex;
    //UIControl uiControl;
    Vector2 cameraGridOffset;
    Vector2 matrixIndexPosition;
	Vector3 pointerScreenPosition, pointerOffset, scrollFadingOffset;

    void Assignments()
    {
        camera = GetComponent<Camera>();
        game = GetComponent<Game>();
        grid = GameObject.Find("Grid");
        menuBar = GameObject.Find("MenuBar");
        cameraGridOffset = transform.position - grid.transform.position;

        // Player zuweisen

        player = GameObject.Find("Player");

        // Buttons zuweisen

        for (int i = 0; i < buttonsCount; i++)
        {
            button[i] = GameObject.Find(buttonName[i]).GetComponent<Button>();
        }

        // Sonstige Zuweisungen

        captionLevel = GameObject.Find("CaptionLevel");
    }

    void BuildConveyor()
    {
        int xOffset = 0;

        if (!pointerReleased)
        {
            int tileIDright = GetTileFromMatrix(matrixIndex.x + 1, matrixIndex.y, -1);
            int tileIDbehind = GetTileFromMatrix(matrixIndex.x, matrixIndex.y, -1);
            int tileIDleft = GetTileFromMatrix(matrixIndex.x - 1, matrixIndex.y, -1);

            if (!moveMode & (tileIDbehind < 270 | tileIDbehind > 282))
            {
                if ((tileIDright == 270 | tileIDright == 272) & (tileIDleft == 272 | tileIDleft == 274))
                {
                    PutTileToMatrix(272, matrixIndex.x, matrixIndex.y, 0);

                    if (tileIDright == 270)
                    {
                        PutTileToMatrix(272, matrixIndex.x + 1, matrixIndex.y, 0);
                        PutTileToMatrix(272, matrixIndex.x + 2, matrixIndex.y, 0);
                    }

                    if (tileIDleft == 274)
                    {
                        PutTileToMatrix(272, matrixIndex.x - 1, matrixIndex.y, 0);
                    }
                }
                else
                {
                    if (tileIDright == 270)
                    {
                        PutTileToMatrix(270, matrixIndex.x + 0, matrixIndex.y, 0);
                        PutTileToMatrix(variationItemID, matrixIndex.x + 1, matrixIndex.y, 0);
                        PutTileToMatrix(272, matrixIndex.x + 2, matrixIndex.y, 0);
                    }
                    else
                    {
                        if (tileIDleft == 274)
                        {
                            PutTileToMatrix(274, matrixIndex.x - 0, matrixIndex.y, 0);
                            PutTileToMatrix(272, matrixIndex.x - 1, matrixIndex.y, 0);
                        }
                        else
                        {
                            if (matrixIndex.x < matrixWidth - 2)
                            {
                                xOffset = 0;
                            }
                            else
                            {
                                xOffset = 2;
                            }

                            PutTileToMatrix(270, matrixIndex.x + 0 - xOffset, matrixIndex.y, 0);
                            PutTileToMatrix(variationItemID, matrixIndex.x + 1 - xOffset, matrixIndex.y, 0);
                            PutTileToMatrix(274, matrixIndex.x + 2 - xOffset, matrixIndex.y, 0);
                        }
                    }
                }

                drawMode = true;
            }
            else
            {
                if (!drawMode & !moveMode)
                {
                    GetConveyor();
                    DeleteItemFromMatrix(matrixIndex.x - item.xOffset, matrixIndex.y, 0);
                    PutItemToMatrix(matrixIndex.x - item.xOffset, matrixIndex.y, 2);
                    itemEditingAllowed = true;
                    moveMode = true;
                }
            }
        }
        else
        {
            if (moveMode)
            {
                if (!itemDragged & itemEditingAllowed)
                {
                    variationItemID = (280 + 282) - item.tileID[1, 0];
                    item.tileID[1, 0] = variationItemID;
                }

                DeleteItemFromMatrix(matrixIndex.x - item.xOffset, matrixIndex.y, 2);
                PutItemToMatrix(matrixIndex.x - item.xOffset, matrixIndex.y, 0);
            }
        }
    }

    void BuildConveyorSwitch()
    {
        if (!pointerReleased)
        {
            bool differentTileIDs = false;
            int itemID = GetTileFromMatrix(matrixIndex.x, matrixIndex.y, 1);

            if (!moveMode)
            {
                if (itemID != 284)
                {
                    differentTileIDs = true;
                }
                else
                {
                    itemEditingAllowed = true;
                }

                if (differentTileIDs)
                {
                    itemID = currentItemID;
                }
                else
                {
                    DeleteTileFromMatrix(matrixIndex.x, matrixIndex.y, 1);
                }

                item.layer = 1;
                InitiateItem(1, 1, matrixIndex.x, matrixIndex.y, 0, 0);
                item.tileID[0, 0] = itemID;
                PutTileToMatrix(itemID, matrixIndex.x, matrixIndex.y, 2);
                moveMode = true;
            }
        }
        else
        {
//            if (!itemDragged & itemEditingAllowed)
//            {
//                item.tileID[0, 0] = (238 + 240) - item.tileID[0, 0];
//                currentItemID = item.tileID[0, 0];
//            }

            DeleteTileFromMatrix(matrixIndex.x, matrixIndex.y, 2);
            PutTileToMatrix(item.tileID[0, 0], matrixIndex.x, matrixIndex.y, 1);
            itemEditingAllowed = false;
        }
    }

    void BuildDoor()
    {
        if (!pointerReleased)
        {
            bool differentTileIDs = false;
            int itemID = 0;
            int xOffset = 0;
            int yOffset = 0;

            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    itemID = GetTileFromMatrix(matrixIndex.x - x, matrixIndex.y - y, 0);

                    if (itemID == currentItemID)
                    {
                        xOffset = x;
                        yOffset = y;
                        goto BuildDoor1;
                    }
                }
            }
            
            BuildDoor1:

            if (!moveMode)
            {
                if (itemID != currentItemID)
                {
                    differentTileIDs = true;
                }

                item.layer = 0;
                InitiateItem(3, 3, matrixIndex.x - xOffset, matrixIndex.y - item.yOffset, xOffset, yOffset);
                item.tileID[0, 0] = currentItemID;
                DeleteTileFromMatrix(matrixIndex.x - xOffset, matrixIndex.y - yOffset, 0);
                PutTileToMatrix(currentItemID, matrixIndex.x - xOffset, matrixIndex.y - yOffset, 2);
                itemEditingAllowed = true;
                moveMode = true;
            }
        }
        else
        {
            if (!itemDragged & itemEditingAllowed)
            {
                
            }

            DeleteTileFromMatrix(matrixIndex.x - item.xOffset, matrixIndex.y - item.yOffset, 2);
            PutTileToMatrix(item.tileID[0, 0], matrixIndex.x - item.xOffset, matrixIndex.y - item.yOffset, 0);
            itemEditingAllowed = false;
        }
    }

    void BuildAndDeleteFloor(bool build, int xMatrix, int yMatrix)
    {
        if (!pointerReleased)
        {
            if (build)
            {
                DetermineProperWallPiece(xMatrix, yMatrix);
            }
            else
            {
                DeleteTileFromMatrix(xMatrix, yMatrix, 0);
            }

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (j != 0 | i != 0)
                    {
                        int spriteID = GetTileFromMatrix(xMatrix + j, yMatrix + i, 0);

                        if (spriteID >= 100 & spriteID <= 222)
                        {
                            DetermineProperWallPiece(xMatrix + j, yMatrix + i);
                        }
                    }
                }
            }
        }
    }

    void BuildLadder()
    {
        int yOffset = 0;

        if (!pointerReleased)
        {
            int tileIDabove = GetTileFromMatrix(matrixIndex.x, matrixIndex.y - 1, -1);
            int tileIDbehind = GetTileFromMatrix(matrixIndex.x, matrixIndex.y, -1);
            int tileIDbelow = GetTileFromMatrix(matrixIndex.x, matrixIndex.y + 1, -1);

            if (!moveMode & (tileIDbehind < 230 | tileIDbehind > 234))
            {
                if ((tileIDbelow == 230 | tileIDbelow == 232) & (tileIDabove == 232 | tileIDabove == 234))
                {
                    PutTileToMatrix(232, matrixIndex.x, matrixIndex.y, 1);

                    if (tileIDbelow == 230)
                    {
                        PutTileToMatrix(232, matrixIndex.x, matrixIndex.y + 1, 1);
                    }

                    if (tileIDabove == 234)
                    {
                        PutTileToMatrix(232, matrixIndex.x, matrixIndex.y - 1, 1);
                    }
                }
                else
                {
                    if (tileIDbelow == 230)
                    {
                        PutTileToMatrix(230, matrixIndex.x, matrixIndex.y + 0, 1);
                        PutTileToMatrix(232, matrixIndex.x, matrixIndex.y + 1, 1);
                    }
                    else
                    {
                        if (tileIDabove == 234)
                        {
                            PutTileToMatrix(234, matrixIndex.x, matrixIndex.y - 0, 1);
                            PutTileToMatrix(232, matrixIndex.x, matrixIndex.y - 1, 1);
                        }
                        else
                        {
                            if (matrixIndex.y < matrixHeight - 1 & (tileIDbelow < 100 | tileIDbelow > 228))
                            {
                                PutTileToMatrix(230, matrixIndex.x, matrixIndex.y, 1);

                                if (GetTileFromMatrix(matrixIndex.x, matrixIndex.y + 2, -1) == 230)
                                {
                                    PutTileToMatrix(232, matrixIndex.x, matrixIndex.y + 1, 1);
                                    PutTileToMatrix(232, matrixIndex.x, matrixIndex.y + 2, 1);
                                }
                                else
                                {
                                    PutTileToMatrix(234, matrixIndex.x, matrixIndex.y + 1, 1);
                                }
                            }
                            else
                            {
                                PutTileToMatrix(234, matrixIndex.x, matrixIndex.y, 1);

                                if (GetTileFromMatrix(matrixIndex.x, matrixIndex.y - 2, -1) == 234)
                                {
                                    PutTileToMatrix(232, matrixIndex.x, matrixIndex.y - 1, 1);
                                    PutTileToMatrix(232, matrixIndex.x, matrixIndex.y - 2, 1);
                                }
                                else
                                {
                                    PutTileToMatrix(230, matrixIndex.x, matrixIndex.y - 1, 1);
                                }
                            }
                        }
                    }
                }

                drawMode = true;
            }
            else
            {
                if (!drawMode & !moveMode)
                {
                    GetLadder();
                    DeleteItemFromMatrix(matrixIndex.x, matrixIndex.y - item.yOffset, 1);
                    PutItemToMatrix(matrixIndex.x, matrixIndex.y - item.yOffset, 2);
                    moveMode = true;
//                    Debug.Log("Get Ladder Item");
                }
            }
        }
        else
        {
            if (moveMode)
            {
            DeleteItemFromMatrix(matrixIndex.x, matrixIndex.y - item.yOffset, 2);
            PutItemToMatrix(matrixIndex.x, matrixIndex.y - item.yOffset, 1);
//                Debug.Log("Put Ladder Item");
            }
        }
    }

    void BuildSpikes()
    {
        if (!pointerReleased)
        {
            PutTileToMatrix(currentItemID, matrixIndex.x, matrixIndex.y, 0);
        }
    }

    void BuildSwitchblock()
    {
        if (!pointerReleased)
        {
            bool differentTileIDs = false;
            int itemID = GetTileFromMatrix(matrixIndex.x, matrixIndex.y, 0);

            if (!drawMode & !moveMode)
            {
                if (itemID != currentItemID)
                {
                    differentTileIDs = true;
                }

                if (!differentTileIDs)
                {
                    item.layer = 0;
                    InitiateItem(1, 1, matrixIndex.x, matrixIndex.y, 0, 0);
                    item.tileID[0, 0] = itemID;
                    DeleteTileFromMatrix(matrixIndex.x, matrixIndex.y, 0);
                    PutTileToMatrix(currentItemID, matrixIndex.x, matrixIndex.y, 2);
                    itemEditingAllowed = true;
                    moveMode = true;
                }
                else
                {
                    drawMode = true;
                    itemEditingAllowed = false;
                }
            }

            if (drawMode)
            {
                PutTileToMatrix(currentItemID, matrixIndex.x, matrixIndex.y, 0);
            }
        }
        else
        {
            if (itemEditingAllowed)
            {
                if (!itemDragged)
                {                    
                    item.tileID[0, 0] = (224 + 226) - item.tileID[0, 0];
                    currentItemID = item.tileID[0, 0];
                }
            
                DeleteTileFromMatrix(matrixIndex.x, matrixIndex.y, 2);
                PutTileToMatrix(item.tileID[0, 0], matrixIndex.x, matrixIndex.y, 0);
                itemEditingAllowed = false;
            }
        }
    }

    void BuildSpring()
    {
        if (!pointerReleased)
        {
            bool differentTileIDs = false;
            int itemID = GetTileFromMatrix(matrixIndex.x, matrixIndex.y, 0);

            if (!moveMode)
            {
                if (itemID != 238 & itemID != 240)
                {
                    differentTileIDs = true;
                }
                else
                {
                    itemEditingAllowed = true;
                }

                if (differentTileIDs)
                {
                    itemID = currentItemID;
                }
                else
                {
                    DeleteTileFromMatrix(matrixIndex.x, matrixIndex.y, 0);
                }

                item.layer = 0;
                InitiateItem(1, 1, matrixIndex.x, matrixIndex.y, 0, 0);
                item.tileID[0, 0] = itemID;
                PutTileToMatrix(itemID, matrixIndex.x, matrixIndex.y, 2);
                moveMode = true;
            }
        }
        else
        {
            if (!itemDragged & itemEditingAllowed)
            {
                item.tileID[0, 0] = (238 + 240) - item.tileID[0, 0];
                currentItemID = item.tileID[0, 0];
            }

            DeleteTileFromMatrix(matrixIndex.x, matrixIndex.y, 2);
            PutTileToMatrix(item.tileID[0, 0], matrixIndex.x, matrixIndex.y, 0);
            itemEditingAllowed = false;
        }
    }

    void BuildSteelbeam()
    {
        if (!pointerReleased)
        {
            int tileID = GetTileFromMatrix(matrixIndex.x, matrixIndex.y, -1);

            if (!moveMode & tileID != 228)
            {
                //Debug.Log(tileID);
                PutTileToMatrix(currentItemID, matrixIndex.x, matrixIndex.y, 0);
                drawMode = true;
            }
            else
            {
                if (!moveMode & !drawMode)
                {
                    GetSteelbeam();
                    DeleteItemFromMatrix(matrixIndex.x - item.xOffset, matrixIndex.y, 0);
                    PutItemToMatrix(matrixIndex.x - item.xOffset, matrixIndex.y, 2);
                    moveMode = true;
                }
            }
        }
        else
        {
            if (moveMode)
            {
                DeleteItemFromMatrix(matrixIndex.x - item.xOffset, matrixIndex.y, 2);
                PutItemToMatrix(matrixIndex.x - item.xOffset, matrixIndex.y, 0);
            }
        }
    }

    void CorrectCameraPosition()
    {
        bool corrected = false;
        float viewUnitsHeight = camera.orthographicSize * 2;
        float viewUnitsWidth = viewUnitsHeight * displayRatio;
        Vector3 cameraPosition = transform.position;

        // Wenn Sichtfeld über rechten Matrix-Rand hinaus

        if (transform.position.x > -viewUnitsWidth / 2 + matrixOffset.x + matrixWidth * tileSize)
        {
            cameraPosition.x = -viewUnitsWidth / 2 + matrixOffset.x + matrixWidth * tileSize;
            corrected = true;
        }

        // Wenn über linken Matrix-Rand hinaus

        if (transform.position.x < viewUnitsWidth / 2 + matrixOffset.x)
        {
            cameraPosition.x = viewUnitsWidth / 2 + matrixOffset.x;
            corrected = true;
        }

        // Wenn über unteren Rand hinaus

        if (transform.position.y < viewUnitsHeight / 2 + matrixOffset.y - matrixHeight * tileSize)
        {
            cameraPosition.y = viewUnitsHeight / 2 + matrixOffset.y - matrixHeight * tileSize;
            corrected = true;
        }

        // Wenn über oberen Rand

        if (transform.position.y > -viewUnitsHeight / 2 + matrixOffset.y)
        {
            cameraPosition.y = -viewUnitsHeight / 2 + matrixOffset.y;
            corrected = true;
        }

        if (corrected)
        {
            transform.position = cameraPosition;
        }
    }

    void CorrectGrid()
    {
        float viewUnitsHeight = camera.orthographicSize * 2;
        float viewUnitsWidth = viewUnitsHeight * displayRatio;
        
        // Wenn Sichtfeld über rechten Raster-Rand hinaus

        if (transform.position.x + viewUnitsWidth / 2 > grid.transform.position.x + gridWidth / 2)
        {
            grid.transform.Translate(Vector3.right * gridWidth / 3);
        }

        // Wenn über linken Raster-Rand hinaus

        if (transform.position.x - viewUnitsWidth / 2 < grid.transform.position.x - gridWidth / 2)
        {
            grid.transform.Translate(Vector3.left * gridWidth / 3);
        }

        // Wenn über unteren Rand hinaus

        if (transform.position.y - viewUnitsHeight / 2 < grid.transform.position.y - gridHeight / 2)
        {
            grid.transform.Translate(Vector3.down * gridHeight / 3);
        }

        // Wenn über oberen Rand

        if (transform.position.y + viewUnitsHeight / 2 > grid.transform.position.y + gridHeight / 2)
        {
            grid.transform.Translate(Vector3.up * gridHeight / 3);
        }
    }

    void CreateMatrix()
    {
        for (int i = 0; i < matrixHeight; i++)
        {
            for (int j = 0; j < matrixWidth; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    tileContainer = new GameObject("Tile");
                    tileContainer.transform.position = new Vector3(matrixOffset.x + tileSize / 2 + j * tileSize, matrixOffset.y - tileSize / 2 - i * tileSize, 2 - k);
                    tileContainer.transform.localScale = new Vector3(100, 100, 1);
                    tileContainer.AddComponent<SpriteRenderer>();
                    matrix[j, i, k].tileContainer = tileContainer;
                    //                    if (k == 0)
                    //                    {
                    //                        matrix[j, i, k].spriteRenderer.sprite = sprite[54];
                    //                    }
                }
            }
        }
        //matrix[3, 3, 0].spriteRenderer.sprite = sprite[spriteIDs[100]];
    }

    void DeleteConveyor(int matrixX, int matrixY)
    {
        int tileID = 0;
        //int tileIDbehind = GetTileFromMatrix(matrixX, matrixY, 0);
        int tileIDright = GetTileFromMatrix(matrixX + 1, matrixY, 0);
        int tileIDright2 = GetTileFromMatrix(matrixX + 2, matrixY, 0);
        int tileIDleft = GetTileFromMatrix(matrixX - 1, matrixY, 0);
        int tileIDleft2 = GetTileFromMatrix(matrixX - 2, matrixY, 0);
        int x = matrixX + 1;

        while (GetTileFromMatrix(x, matrixY, 0) != 270)
        {
            tileID = GetTileFromMatrix(x, matrixY, 0);

            if (tileID == 280 | tileID == 282)
            {
                break;
            }

            x--;
        }

        DeleteTileFromMatrix(matrixX, matrixY, 0);

        if (tileIDright == 274)
        {
            DeleteTileFromMatrix(matrixX + 1, matrixY, 0);
        }
        else
        {
            if (tileIDright2 == 274)
            {
                DeleteTileFromMatrix(matrixX + 1, matrixY, 0);
                DeleteTileFromMatrix(matrixX + 2, matrixY, 0);
            }
        }

        if (tileIDleft == 270)
        {
            DeleteTileFromMatrix(matrixX - 1, matrixY, 0);
        }
        else
        {
            if (tileIDleft2 == 270)
            {
                DeleteTileFromMatrix(matrixX - 1, matrixY, 0);
                DeleteTileFromMatrix(matrixX - 2, matrixY, 0);
            }
        }

        if ((tileIDright == 272 | tileIDright == 280 | tileIDright == 282) & tileIDright2 == 272)
        {
            PutTileToMatrix(270, matrixX + 1, matrixY, 0);
            PutTileToMatrix(tileID, matrixX + 2, matrixY, 0);
        }

        if (tileIDleft == 272)
        {
            PutTileToMatrix(274, matrixX - 1, matrixY, 0);
        }
    }

    void DeleteItem(int matrixX, int matrixY)
    {
        int itemID = GetTileFromMatrix(matrixIndex.x, matrixIndex.y, -1);

        if (itemID >= 230 & itemID <= 234)
        {
            DeleteLadder(matrixX, matrixY);
        }
        else if (itemID >= 100 & itemID <= 222)
        {
            pointerReleased = false;
            BuildAndDeleteFloor(false, matrixX, matrixY);
        }
        else if (itemID >= 270 & itemID <= 282)
        {
            DeleteConveyor(matrixX, matrixY);
        }
        else
        {
            DeleteTileFromMatrix(matrixIndex.x, matrixIndex.y, -1);
        }
    }

    void DeleteLadder(int matrixX, int matrixY)
    {
        int tileIDabove = GetTileFromMatrix(matrixX, matrixY - 1, 1);
        int tileIDbelow = GetTileFromMatrix(matrixX, matrixY + 1, 1);

        DeleteTileFromMatrix(matrixX, matrixY, 1);

        if (tileIDabove == 232)
        {
            PutTileToMatrix(234, matrixX, matrixY - 1, 1);
        }
        else
        {
            if (tileIDabove == 230)
            {
                DeleteTileFromMatrix(matrixX, matrixY - 1, 1);
            }
        }

        if (tileIDbelow == 232)
        {
            PutTileToMatrix(230, matrixX, matrixY + 1, 1);
        }
        else
        {
            if (tileIDbelow == 234)
            {
                DeleteTileFromMatrix(matrixX, matrixY + 1, 1);
            }
        }
    }

    void DeleteItemFromMatrix(int x, int y, int layer)
    {
        int itemID;
        int width = item.tileID.GetLength(0);
        int height = item.tileID.GetLength(1);

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                itemID = item.tileID[j, i];

                if (itemID != 0)
                {
                    DeleteTileFromMatrix(item.x + j, item.y + i, layer);
                }
            }
        }
    }

    void DeleteTileFromMatrix(int x, int y, int layer)
    {
        if (x >= 0 & x < matrixWidth & y >= 0 & y < matrixHeight)
        {
            if (layer != -1)
            {
                PutTileToMatrix(0, x, y, layer);
            }
            else
            {
                PutTileToMatrix(0, x, y, 0);
                PutTileToMatrix(0, x, y, 1);
            }
        }
    }

    void DetermineProperWallPiece(int xMatrix, int yMatrix)
    {
        bool[,] isWall = new bool[3, 3];
        string tileName;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int itemID = GetTileFromMatrix(xMatrix - 1 + j, yMatrix - 1 + i, 0);

                if (itemID >= 100 & itemID <= 222)
                {
                    isWall[j, i] = true;
                }
            }
        }

        if (!isWall[1, 0])
        {
            tileName = "Floor";
        }
        else
        {
            tileName = "Wall";
        }

        if (!isWall[1, 2])
        {
            tileName = tileName + " S";
        }

        if (!isWall[2, 1])
        {
            tileName = tileName + " E";
        }

        if (!isWall[0, 1])
        {
            tileName = tileName + " W";
        }

        if (isWall[1, 0] & isWall[0, 1] & !isWall[0, 0] | isWall[1, 0] & isWall[2, 1] & !isWall[2, 0] | isWall[0, 1] & isWall[1, 2] & !isWall[0, 2] | isWall[2, 1] & isWall[1, 2] & !isWall[2, 2])
        {
            tileName = tileName + " Inner";

            if (isWall[1, 0] & isWall[0, 1] & !isWall[0, 0])
            {
                tileName = tileName + " NW";
            }

            if (isWall[1, 0] & isWall[2, 1] & !isWall[2, 0])
            {
                tileName = tileName + " NE";
            }

            if (isWall[0, 1] & isWall[1, 2] & !isWall[0, 2])
            {
                tileName = tileName + " SW";
            }

            if (isWall[2, 1] & isWall[1, 2] & !isWall[2, 2])
            {
                tileName = tileName + " SE";
            }

            if (tileName.Substring(tileName.Length - 12) == " NW NE SW SE")
            {
                tileName = tileName.Substring(0, tileName.Length - 12);
            }
        }

        //Debug.Log(tileName);
            
        string[] tileNameParts = tileName.Split(new string[] { " " }, System.StringSplitOptions.None);

        foreach(string listEntry in spriteList)
        {
            //Debug.Log(spriteName);
            int spriteID = int.Parse(listEntry.Substring(0, 3));

            if (spriteID >= 100 & spriteID <= 222)
            {
                string spriteName = listEntry.Substring(4);
                string[] spriteNameParts = spriteName.Split(new string[] { "_" }, System.StringSplitOptions.None);
                spriteName = "_" + spriteName + "_";
                //spriteName += "_";

                if (spriteNameParts.GetLength(0) == tileNameParts.GetLength(0))
                {
                    //Debug.Log(spriteName);
                    bool partNotFound = false;

                    for (int i = 0; i < tileNameParts.GetLength(0); i++)
                    {
                        //Debug.Log("_" + tileNameParts[i] + "_");
                        if (!spriteName.Contains("_" + tileNameParts[i] + "_"))
                        {
                            partNotFound = true;
                        }
                    }

                    if (!partNotFound)
                    {
                        PutTileToMatrix(spriteID, xMatrix, yMatrix, 0);
                        //Debug.Log("...");
                        return;
                    }
                }
            }
        }
    }

    void DisplayCaptionLevel()
    {
        captionLevel.SetActive(true);
        captionLevel.GetComponent<Text>().text = "Level " + currentLevel.ToString();
        CaptionLevelControl.visibilityTimer = 60;
    }

    void Editing()
    {
        bool matrixPositionChanged;

        if (editMode != 0)
        {
            if (Input.GetMouseButton(0) & !editLock)
            {
                if (!pointerState)
                {
                    pointerState = true;
                }
                else
                {
                    pointerScreenPosition = Input.mousePosition;
                    matrixPositionChanged = GetMatrixPosition(pointerScreenPosition);

                    if (editMode != 2)
                    {
                        if (matrixPositionChanged)
                        {
                            pointerReleased = false;
                            GetItemBuildFunction();

                            if (moveMode)
                            {
                                MoveItem();
                            }
                        }
                    }
                    else
                    {
                        DeleteItem(matrixIndex.x, matrixIndex.y);
                    }
                }
            }
            else
            {
                if (pointerState)
                {
                    pointerReleased = true;
                    pointerState = false;
                    drawMode = false;

                    if (editMode != 2)
                    {
                        GetItemBuildFunction();
                    }

                    moveMode = false;
                    matrixIndex.reset = true;
                }
            }
        }
    }

    void EnableScrollFading()
    {
        scrollFading = true;
        scrollFadingOffset.x = 0;
        scrollFadingOffset.y = 0;
    }

    public void Exit()
    {
        SaveLevel();
        Application.Quit();
    }

    void FadeScrolling()
    {
        if (scrollFading)
        {
            scrollFadingOffset.x /= scrollFadingAmount;
            scrollFadingOffset.y /= scrollFadingAmount;
            transform.Translate(-scrollFadingOffset);
            if (Mathf.Abs(scrollFadingOffset.x) < 1 & Mathf.Abs(scrollFadingOffset.y) < 1)
            {
                scrollFading = false;
            }
        }
    }

    void GetConveyor()
    {
        int tileID = 0;
        int x1 = matrixIndex.x;
        int x2;

        while (GetTileFromMatrix(x1, matrixIndex.y, 0) != 270)
        {
            x1--;
        }

        x2 = x1;

        while (GetTileFromMatrix(x2, matrixIndex.y, 0) != 274)
        {
            x2++;
        }

        InitiateItem(x2 - x1 + 1, 1, x1, matrixIndex.y, matrixIndex.x - x1, 0);
        item.tileID[0, 0] = 270;
        item.tileID[1, 0] = GetTileFromMatrix(x1 + 1, matrixIndex.y, 0);
        item.tileID[x2 - x1, 0] = 274;

        for (int i = 0; i < x2 - x1 - 2; i++)
        {
            item.tileID[i + 2, 0] = 272;
        }
    }

    void GetItemBuildFunction()
    {
        switch(currentItemID)
        {
            case 100:
                BuildAndDeleteFloor(true, matrixIndex.x, matrixIndex.y);
                break;
            case 224:
                BuildSwitchblock();
                break;
            case 226:
                BuildSwitchblock();
                break;
            case 228:
                BuildSteelbeam();
                break;
            case 232:
                BuildLadder();
                break;
            case 236:
                BuildSpikes();
                break;
            case 238:
                BuildSpring();
                break;
            case 240:
                BuildSpring();
                break;
            case 270:
                BuildConveyor();
                break;
            case 284:
                BuildConveyorSwitch();
                break;
            case 396:
                BuildDoor();
                break;
            case 600:
                PutPlayer();
                break;
            case 601:
                PutPlayer();
                break;
        }
    }

    void GetLadder()
    {
        int tileID = 0;
        int y1 = matrixIndex.y;
        int y2;

        while (GetTileFromMatrix(matrixIndex.x, y1, 1) != 230)
        {
            y1--;
        }

        y2 = y1;

        while (GetTileFromMatrix(matrixIndex.x, y2, 1) != 234)
        {
            y2++;
        }
            
        InitiateItem(1, y2 - y1 + 1, matrixIndex.x, y1, 0, matrixIndex.y - y1);
        item.tileID[0, 0] = 230;
        item.tileID[0, y2 - y1] = 234;

        for (int i = 0; i < y2 - y1 - 1; i++)
        {
            item.tileID[0, i + 1] = 232;
        }
    }

    bool GetMatrixPosition(Vector3 pointerPosition)
    {
        bool changed = false;
        Vector3 pointerWorldPosition = camera.ScreenToWorldPoint(pointerPosition);
    
        matrixIndex.xTarget = (int)Mathf.Floor((pointerWorldPosition.x - matrixOffset.x) / tileSize);
        matrixIndex.yTarget = (int)Mathf.Floor(-(pointerWorldPosition.y - matrixOffset.y) / tileSize);

        matrixIndex.xTarget = Mathf.Clamp(matrixIndex.xTarget, 0, matrixWidth);
        matrixIndex.yTarget = Mathf.Clamp(matrixIndex.yTarget, 0, matrixHeight);

        if (matrixIndex.reset)
        {
            matrixIndex.reset = false;
            matrixIndex.x = matrixIndex.xTarget;
            matrixIndex.y = matrixIndex.yTarget;
            itemDragged = false;
            return true;
        }
        else
        {
            if (matrixIndex.x < matrixIndex.xTarget)
            {
                matrixIndex.x++;
                changed = true;
            }

            if (matrixIndex.x > matrixIndex.xTarget)
            {
                matrixIndex.x--;
                changed = true;
            }
            

            if (matrixIndex.y < matrixIndex.yTarget)
            {
                matrixIndex.y++;
                changed = true;
            }

            if (matrixIndex.y > matrixIndex.yTarget)
            {
                matrixIndex.y--;
                changed = true;
            }

            if (changed)
            {
                itemDragged = true;
                return true;
            }
        }

        return false;
    }

    void GetSteelbeam()
    {
        int tileID = 0;
        int x1 = matrixIndex.x;
        int x2;

        while (GetTileFromMatrix(x1 - 1, matrixIndex.y, 0) == 228)
        {
            x1--;
        }

        x2 = x1;

        while (GetTileFromMatrix(x2 + 1, matrixIndex.y, 0) == 228)
        {
            x2++;
        }

        InitiateItem(x2 - x1 + 1, 1, x1, matrixIndex.y, matrixIndex.x - x1, 0);

        for (int i = 0; i < x2 - x1 + 1; i++)
        {
            item.tileID[i, 0] = 228;
        }
    }

    int GetTileFromMatrix(int x, int y, int layer)
    {
        if (x >= 0 & x < matrixWidth & y >= 0 & y < matrixHeight)
        {
            if (layer != -1)
            {
                return matrix[x, y, layer].spriteID;
            }
            else
            {
                if (matrix[x, y, 1].spriteID != 0)
                {
                    return matrix[x, y, 1].spriteID;
                }
                else
                {
                    return matrix[x, y, 0].spriteID;
                }
            }
        }
        else
        {
            return 0;
        }
    }

    void InitiateGame()
    {
        int spriteID;
        string tag = "";

        // Analyze Matrix

        for (int i = 0; i < matrixHeight; i++)
        {
            for (int j = 0; j < matrixWidth; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    spriteID = matrix[j, i, k].spriteID;

                    if (spriteID > 0)
                    {
                        // Add Tags & Colliders

                        matrix[j, i, k].tileContainer.AddComponent<BoxCollider2D>();

                        if (spriteID >= 100 & spriteID <= 222 | spriteID >= 226 & spriteID <= 228)
                        {
                            tag = "Wall";
                        }

                        if (spriteID >= 230 & spriteID <= 234)
                        {
                            tag = "Ladder";
                        }

                        if (spriteID == 236)
                        {
                            tag = "Spikes";
                        }

                        if (spriteID >= 238 & spriteID <= 240)
                        {
                            tag = "Spring";
                        }

                        if (spriteID >= 270 & spriteID <= 280)
                        {
                            tag = "Conveyor";
                        }

                        if (spriteID >= 396 & spriteID <= 399)
                        {
                            tag = "Door";
                        }

                        if (tag != "")
                        {
                            matrix[j, i, k].tileContainer.tag = tag;
                        }

                        // Hide Player-Dummies & Place Player Object

                        if (spriteID >= 600 & spriteID <= 601)
                        {
                            matrix[j, i, k].tileContainer.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    void InitiateItem(int width, int height, int x, int y, int xOffset, int yOffset)
    {
        item.tileID = new int[width, height];
        item.x = x;
        item.y = y;
        item.xOffset = xOffset;
        item.yOffset = yOffset;
    }

    void InitiateVariables()
    {
        // Liste aller Buttonnamen erstellen

        buttonName = new string[]
            {
                "ButtonAdd",
                "ButtonPan",
                "ButtonEraser",
                "ButtonMagnifier+",
                "ButtonMagnifier-",
                "ButtonGrid",
                "ButtonPlay",
                "ButtonUndo",
                "ButtonRedo",
                "ButtonSettings",
                "ButtonExit"
            };

        // Liste aller Spritenamen erstellen

        spriteList = new string[]
            {
                "001 Button_Add_0",
                "000 Button_Add_1",
                "002 Button_Eraser_0",
                "000 Button_Eraser_1",
                "003 Button_Exit_0",
                "000 Button_Exit_1",
                "004 Button_Grid_0",
                "000 Button_Grid_1",
                "005 Button_Magnifier-_0",
                "000 Button_Magnifier-_1",
                "006 Button_Magnifier+_0",
                "000 Button_Magnifier+_1",
                "008 Button_Normal",
                "007 Button_Pan_0",
                "000 Button_Pan_1",
                "009 Button_Play_0",
                "000 Button_Play_1",
                "010 Button_Pressed",
                "011 Button_Redo_0",
                "000 Button_Redo_1",
                "012 Button_Settings_0",
                "000 Button_Settings_1",
                "013 Button_Stop_0",
                "000 Button_Stop_1",
                "014 Button_Undo_0",
                "000 Button_Undo_1",
                "280 Conveyor_Arrow_Left",
                "282 Conveyor_Arrow_Right",
                "270 Conveyor_L_0",
                "000 Conveyor_L_1",
                "000 Conveyor_L_2",
                "000 Conveyor_L_3",
                "272 Conveyor_M_0",
                "000 Conveyor_M_1",
                "000 Conveyor_M_2",
                "000 Conveyor_M_3",
                "274 Conveyor_R_0",
                "000 Conveyor_R_1",
                "000 Conveyor_R_2",
                "000 Conveyor_R_3",
                "284 Conveyor_Switch",
                "286 Conveyor_Switch_Left",
                "288 Conveyor_Switch_Pressed",
                "290 Conveyor_Switch_Right",
                "396 Door_TypeA",
                "110 Floor",
                "102 Floor_E",
                "104 Floor_E_Inner_SW",
                "120 Floor_E_W",
                "100 Floor_E_W_S",
                "112 Floor_Inner_SE",
                "114 Floor_Inner_SE_SW",
                "116 Floor_Inner_SW",
                "108 Floor_S",
                "118 Floor_S_E",
                "106 Floor_S_W",
                "122 Floor_W",
                "124 Floor_W_Inner_SE",
                "000 Grid",
                "232 Ladder",
                "234 Ladder_End_B",
                "230 Ladder_End_T",
                "250 Pipe_Bracket_NE",
                "256 Pipe_Bracket_NW",
                "252 Pipe_Bracket_SE",
                "254 Pipe_Bracket_SW",
                "262 Pipe_End_E",
                "264 Pipe_End_N",
                "268 Pipe_End_S",
                "258 Pipe_End_W",
                "260 Pipe_Straight_H",
                "266 Pipe_Straight_V",
                "000 Player_Climb_0",
                "000 Player_Climb_1",
                "000 Player_Idle_Left",
                "000 Player_Idle_Right",
                "000 Player_Stand",
                "000 Player_Walk_Left_0",
                "000 Player_Walk_Left_1",
                "000 Player_Walk_Right_0",
                "000 Player_Walk_Right_1",
                "601 Player-Dummy_Idle_Left",
                "600 Player-Dummy_Idle_Right",
                "200 Slope_Flat_Left",
                "202 Slope_Flat_Left_R",
                "204 Slope_Flat_Right",
                "206 Slope_Flat_Right_L",
                "208 Slope_Start_Flat_Left",
                "210 Slope_Start_Flat_Right",
                "212 Slope_Start_Steep_Left",
                "214 Slope_Start_Steep_Right",
                "216 Slope_Steep_Left",
                "218 Slope_Steep_Left_R",
                "220 Slope_Steep_Right",
                "222 Slope_Steep_Right_L",
                "236 Spikes",
                "238 Spring_TypeA_0",
                "000 Spring_TypeA_1",
                "240 Spring_TypeB_0",
                "000 Spring_TypeB_1",
                "228 Steelbeam",
                "224 Switchfloor_Disabled",
                "226 Switchfloor_Enabled",
                "126 Wall",
                "130 Wall_E",
                "132 Wall_E_Inner_NW",
                "134 Wall_E_Inner_NW_SW",
                "136 Wall_E_Inner_SW",
                "138 Wall_E_W",
                "128 Wall_E_W_S",
                "140 Wall_Inner",
                "142 Wall_Inner_NE",
                "144 Wall_Inner_NE_NW",
                "146 Wall_Inner_NE_NW_SE",
                "148 Wall_Inner_NE_NW_SW",
                "150 Wall_Inner_NE_SE",
                "152 Wall_Inner_NE_SE_SW",
                "154 Wall_Inner_NE_SW",
                "156 Wall_Inner_NW",
                "158 Wall_Inner_NW_SE",
                "160 Wall_Inner_NW_SE_SW",
                "162 Wall_Inner_NW_SW",
                "164 Wall_Inner_SE",
                "166 Wall_Inner_SE_SW",
                "168 Wall_Inner_SW",
                "170 Wall_S",
                "178 Wall_S_E",
                "180 Wall_S_E_Inner_NW",
                "172 Wall_S_Inner_NE",
                "174 Wall_S_Inner_NE_NW",
                "176 Wall_S_Inner_NW",
                "182 Wall_S_W",
                "184 Wall_S_W_Inner_NE",
                "186 Wall_W",
                "188 Wall_W_Inner_NE",
                "190 Wall_W_Inner_NE_SE",
                "192 Wall_W_Inner_SE",
            };

        // Variablen setzen

        buttonsCount = buttonName.GetLength(0);
        spritesCount = spriteList.GetLength(0);
        button = new Button[buttonsCount];
        sprite = new Sprite[spritesCount];
        spriteIDs = new int[800];
        matrix = new smatrix[matrixWidth, matrixHeight, 3];
        matrixIndex.reset = true;
        matrixOffset.x = -512;
        matrixOffset.y = 288;
        editLock = true;
        scrollFadingAmount = General.scrollFadingAmount;
    }

    void LoadLevel(int level)
    {
        int itemID;

        if (level == 0)
        {
            currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        }
        else
        {
            currentLevel = level;
        }

        if (currentLevel == 0)
        {
            currentLevel = 1;
        }

        string levelName = "Level" + currentLevel.ToString();

        if (PlayerPrefs.HasKey(levelName + "Matrix"))
        {
            string matrixString = PlayerPrefs.GetString(levelName + "Matrix");
            int readPosition = 0;

            if (matrixString.Length == matrixWidth * matrixHeight * 6)
            {
                for (int i = 0; i < matrixHeight; i++)
                {
                    for (int j = 0; j < matrixWidth; j++)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            itemID = int.Parse(matrixString.Substring(readPosition, 3));
                            PutTileToMatrix(itemID, j, i, k);
                            readPosition += 3;
                        }
                    }
                }
            }
        }
    }

    void LoadSprites()
    {
        int spriteID;

        for (int i = 0; i < spritesCount; i++)
        {
            spriteID = int.Parse(spriteList[i].Substring(0, 3));
            spriteIDs[spriteID] = i;
            sprite[i] = Resources.Load("Sprites/" + spriteList[i].Substring(4), typeof(Sprite)) as Sprite;
        }
    }

    void MoveItem()
    {
        int itemID;
        int width = item.tileID.GetLength(0);
        int height = item.tileID.GetLength(1);

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                itemID = item.tileID[j, i];

                if (itemID != 0)
                {
                    DeleteTileFromMatrix(item.x + j, item.y + i, 2);
                }
            }
        }

        item.x = matrixIndex.x - item.xOffset;
        item.y = matrixIndex.y - item.yOffset;

        item.x = Mathf.Clamp(item.x, 0, matrixWidth - item.tileID.GetLength(0));
        item.y = Mathf.Clamp(item.y, 0, matrixHeight - item.tileID.GetLength(1));

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                itemID = item.tileID[j, i];

                if (itemID != 0)
                {
                    PutTileToMatrix(itemID, item.x + j, item.y + i, 2);
                }
            }
        }
    }

	public void Play()
	{
		if (playState)
		{
            SetButton(buttonPlay, spriteIDs[idButtonPlay], spriteIDs[idButtonPlay] + 1);
            SetEditorMenuBar();
            game.ExitGame();
            General.programState = 0;
            camera.orthographicSize = cameraSize;
            grid.GetComponent<SpriteRenderer>().enabled = gridVisibility;
        }
		else
		{
            SetButton(buttonPlay, spriteIDs[idButtonStop], spriteIDs[idButtonStop] + 1);
            SetPreviewPlayingMenuBar();
            game.InitiateGame();
            General.programState = 1;
		}
		playState = !playState;
	}

    void PutPlayer()
    {
        if (!pointerReleased)
        {
            bool differentTileIDs = false;
            int itemID = GetTileFromMatrix(matrixIndex.x, matrixIndex.y, 1);

            if (!moveMode)
            {
                if (itemID != 600 & itemID != 601)
                {
                    differentTileIDs = true;
                }

                if(differentTileIDs)
                {
                    itemID = currentItemID;
                }
                else
                {
                    DeleteTileFromMatrix(matrixIndex.x, matrixIndex.y, 1);
                    itemEditingAllowed = true;
                }
                    
                item.layer = 1;
                InitiateItem(1, 1, matrixIndex.x, matrixIndex.y, 0, 0);
                item.tileID[0, 0] = itemID;
                PutTileToMatrix(currentItemID, matrixIndex.x, matrixIndex.y, 2);
                moveMode = true;
            }
        }
        else
        {
            if (!itemDragged & itemEditingAllowed)
            {
                item.tileID[0, 0] = (600 + 601) - item.tileID[0, 0];
                currentItemID = item.tileID[0, 0];
            }

            DeleteTileFromMatrix(matrixIndex.x, matrixIndex.y, 2);
            PutTileToMatrix(item.tileID[0, 0], matrixIndex.x, matrixIndex.y, 1);
            itemEditingAllowed = false;
        }
    }

    void PutItemToMatrix(int x, int y, int layer)
    {
        int itemID;
        int width = item.tileID.GetLength(0);
        int height = item.tileID.GetLength(1);

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                itemID = item.tileID[j, i];

                if (itemID != 0)
                {
                    PutTileToMatrix(item.tileID[j, i], item.x + j, item.y + i, layer);
                }
            }
        }
    }

    void PutTileToMatrix(int itemSpriteID, int x, int y, int layer)
    {
        if (x >= 0 & x < matrixWidth & y >= 0 & y < matrixHeight)
        {
            if (matrix[x, y, layer].spriteID != itemSpriteID)
            {
                matrix[x, y, layer].spriteID = itemSpriteID;

                if (itemSpriteID != 0)
                {
                    matrix[x, y, layer].tileContainer.GetComponent<SpriteRenderer>().sprite = sprite[spriteIDs[itemSpriteID]];
                }
                else
                {
                    matrix[x, y, layer].tileContainer.GetComponent<SpriteRenderer>().sprite = null;
                }
            }
        }
    }

    void SaveLevel()
    {
        int itemID;
        string itemIDstring;
        string matrixString = "";

        PlayerPrefs.SetInt("CurrentLevel", currentLevel);

        for (int i = 0; i < matrixHeight; i++)
        {
            for (int j = 0; j < matrixWidth; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    itemID = GetTileFromMatrix(j, i, k);
                    itemIDstring = itemID.ToString("000");
                    matrixString += itemIDstring;
                }
            }
        }

        string levelName = "Level" + currentLevel.ToString();
        PlayerPrefs.SetString(levelName + "Matrix", matrixString);
    }

    void Scrolling()
    {
        if (editMode == 0)
        {
            // Mit Pointereingaben scrollen

            if (Input.GetMouseButton(0) & !editLock)
            {
                if (!pointerState)
                {
                    pointerState = true;
                    pointerScreenPosition = Input.mousePosition;
                    Vector3 pointerWorldPositionA = camera.ScreenToWorldPoint(new Vector3(0, 0, 0));
                    Vector3 pointerWorldPositionB = camera.ScreenToWorldPoint(new Vector3(1, 1, 0));
                    screenWorldRatio = pointerWorldPositionB.x - pointerWorldPositionA.x;
                    EnableScrollFading();
                }
                else
                {
                    if (pointerScreenPosition.x != Input.mousePosition.x | pointerScreenPosition.y != Input.mousePosition.y)
                    {
                        pointerOffset = (Input.mousePosition - pointerScreenPosition) * screenWorldRatio;
                        scrollFadingOffset = pointerOffset;
                        pointerScreenPosition = Input.mousePosition;
                        transform.Translate(-pointerOffset);
                    }
                }
            }
            else
            {
                pointerState = false;
                FadeScrolling();
            }

            // Mit Keyboardeingaben scrollen

            pointerOffset = new Vector3(0, 0, 0);

            if (Input.GetKey(KeyCode.RightArrow))
                pointerOffset.x = 10;
            
            if (Input.GetKey(KeyCode.LeftArrow))
                pointerOffset.x = -10;
            
            if (Input.GetKey(KeyCode.DownArrow))
                pointerOffset.y = -10;
            
            if (Input.GetKey(KeyCode.UpArrow))
                pointerOffset.y = 10;

            if (pointerOffset.x != 0 | pointerOffset.y != 0)
            {
                transform.Translate(pointerOffset);
                scrollFading = false;
            }

            // Kamera-Position und Raster-Anzeige korrigieren

            CorrectCameraPosition();
            CorrectGrid();
        }
    }

    void SetButton(int buttonIndex, int spriteIndex, int spritePressedIndex)
    {
        button[buttonIndex].image.sprite = sprite[spriteIndex];
        if (spritePressedIndex != -1)
        {
            buttonSprites.pressedSprite = sprite[spritePressedIndex];
            button[buttonIndex].spriteState = buttonSprites;
        }
    }

    void SetEditorMenuBar()
    {
        menuBar.GetComponent<Image>().enabled = true;

        for (int i = 0; i < buttonsCount; i++)
        {
            if (i != buttonPlay)
            {
                button[i].image.enabled = true;
            }
        }
    }

    public void SetModeAdd()
    {
        if (editMode != 1)
        {
            editMode = 1;
            editModePrevious = 1;
            SetButton(buttonPan, spriteIDs[7], -1);
            SetButton(buttonEraser, spriteIDs[2], -1);
            SetButton(buttonAdd, spriteIDs[1] + 1, -1);
        }
    }

    public void SetModeErase()
    {
        if (editMode == 2)
        {
            editMode = editModePrevious;
            editModePrevious = 2;
            SetButton(buttonEraser, spriteIDs[2], -1);

            if (editMode == 1)
            {
                SetButton(buttonAdd, spriteIDs[1] + 1, -1);
            }
            else
            {
                SetButton(buttonPan, spriteIDs[7] + 1, -1);
            }
        }
        else
        {
            editModePrevious = editMode;
            editMode = 2;
            SetButton(buttonAdd, spriteIDs[1], -1);
            SetButton(buttonPan, spriteIDs[7], -1);
            SetButton(buttonEraser, spriteIDs[2] + 1, -1);
        }
    }

    public void SetModePan()
    {
        if (editMode == 0)
        {
            if (editModePrevious != 0)
            {
                editMode = editModePrevious;
                SetButton(buttonPan, spriteIDs[7], -1);

                if (editModePrevious == 1)
                {
                    SetButton(buttonAdd, spriteIDs[1] + 1, -1);
                }
                else
                {
                    SetButton(buttonEraser, spriteIDs[2] + 1, -1);
                }
            }
        }
        else
        {
            editModePrevious = editMode;
            editMode = 0;
            SetButton(buttonAdd, spriteIDs[1], -1);
            SetButton(buttonEraser, spriteIDs[2], -1);
            SetButton(buttonPan, spriteIDs[7] + 1, -1);
        }
    }

    void SetPreviewPlayingMenuBar()
    {
        for (int i = 0; i < buttonsCount; i++)
        {
            if (i != buttonPlay)
            {
                button[i].image.enabled = false;
            }
        }
        menuBar.GetComponent<Image>().enabled = false;
    }

    // Use this for initialization

    void Start()
    {
        InitiateVariables();
        Assignments();

        // HUD deaktivieren

        game.hud.SetActive(false);

        // Sprites laden

        LoadSprites();

        // Matrix erstellen

        CreateMatrix();

        // Level laden

        LoadLevel(0);
        DisplayCaptionLevel();
    }

    public void SwitchLevel()
    {
        SaveLevel();
        currentLevel++;

        if (currentLevel > 3)
        {
            currentLevel = 1;
        }

        LoadLevel(currentLevel);
        DisplayCaptionLevel();
        //captionLevel.SetActive(!captionLevel.activeSelf);
    }

    public void ToggleGridVisibility()
    {
        gridVisibility = !gridVisibility;
        grid.GetComponent<SpriteRenderer>().enabled = gridVisibility;
    }

    // Update is called once per frame

    void Update()
    {
        if (General.programState == 0)
        {
            Scrolling();
            Editing();
        }
    }

    public void ZoomIn()
    {
        if (zoomLevel > 0)
        {
            camera.orthographicSize -= tileSize * 2;
            cameraSize = camera.orthographicSize;
            zoomLevel--;
            CorrectCameraPosition();
            CorrectGrid();
        }
    }

    public void ZoomOut()
    {
        if (zoomLevel < 2)
        {
            camera.orthographicSize += tileSize * 2;
            cameraSize = camera.orthographicSize;
            zoomLevel++;
            CorrectCameraPosition();
            CorrectGrid();
        }
    }
}