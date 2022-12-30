using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {

    struct smatrixIndex
    {
        public int x;
        public int y;
        public int xTarget;
        public int yTarget;
    }

    public static int gravity = 500;
    public static int levelTime = 180;
    public static int remainingTime;
    public static int startMode = -1;
    public static int walkingSpeed = 100;
    public GameObject hud;
    public GameObject player;
    bool actionState;
    bool pointerState;
    bool scrollFading;
    bool scrollLock;
    float cameraSizeDefault = 288;
    float screenWorldRatio;
    float scrollFadingAmount;
    float timer;
    int matrixWidth, matrixHeight;
    int minutes, seconds;
    int switchblockTargetState;
    int tileSize;
    Camera camera;
    Editor editor;
    smatrixIndex matrixIndex;
    Text time;
    Text precount;
    Vector2 matrixIndexPosition, matrixOffset;
    Vector3 pointerScreenPosition, pointerOffset, scrollFadingOffset;

    List<GameObject> players = new List<GameObject>();

    void Assignments()
    {
        camera = GetComponent<Camera>();
        hud = GameObject.Find("HUD");
        player = GameObject.Find("Player");
        precount = GameObject.Find("PreCount").GetComponent<Text>();
        time = GameObject.Find("Time").GetComponent<Text>();
    }

    void EnableScrollFading()
    {
        scrollFading = true;
        scrollFadingOffset.x = 0;
        scrollFadingOffset.y = 0;
    }

    public void ExitGame()
    {

        // Entferne Player

        for (int i = 0; i < players.Count; i++)
        {
            Destroy(players[i]);
        }

        // Analysiere Matrix

        for (int i = 0; i < Editor.matrixHeight; i++)
        {
            for (int j = 0; j < Editor.matrixWidth; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    int spriteID = Editor.matrix[j, i, k].spriteID;

                    // Entferne Colliders & Tags

                    if (spriteID > 0)
                    {
                        Destroy(Editor.matrix[j, i, k].tileContainer.GetComponent<BoxCollider2D>());
                        Editor.matrix[j, i, k].tileContainer.tag = "Untagged";
                    }

                    // Aktiviere Player-Dummies

                    if (spriteID >= 600 & spriteID <= 601)
                    {
                        Editor.matrix[j, i, k].tileContainer.SetActive(true);
                    }
                }
            }
        }

        //HUD deaktivieren
        
        hud.SetActive(false);
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

    void HUDGetTimeParts(int sec)
    {
        if (sec > 59)
        {
            minutes = sec / 60;
            seconds = sec % 60;
        }
        else
        {
            minutes = 0;
            seconds = sec;
        }
    }

    void HUDUpdatePrecount(bool on)
    {
        if (on)
        {
            if (startMode > 0)
            {
                precount.text = "Start in ..." + startMode.ToString();
            }
            else
            {
                precount.text = "";
            }
        }
        else
        {
            precount.text = "";
        }
    }

    void HUDUpdateTime(bool on)
    {
        if (on)
        {
            HUDGetTimeParts(remainingTime);
            time.text = "Time: " + minutes.ToString() + ":" + seconds.ToString("00");
        }
        else
        {
            time.text = "";
        }
    }

    void InitiateVariables()
    {
        matrixOffset.x = Editor.matrixOffset.x;
        matrixOffset.y = Editor.matrixOffset.y;
        matrixWidth = Editor.matrixWidth;
        matrixHeight = Editor.matrixHeight;
        tileSize = Editor.tileSize;
        scrollFadingAmount = General.scrollFadingAmount;
    }

    public void InitiateGame()
    {
        int playerNo = 0;
        string tag = "";

        // Analysiere Matrix

        for (int i = 0; i < Editor.matrixHeight; i++)
        {
            for (int j = 0; j < Editor.matrixWidth; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    int spriteID = Editor.matrix[j, i, k].spriteID;

                    if (spriteID > 0)
                    {
                        // Fuege Colliders & Tags hinzu

                        Editor.matrix[j, i, k].tileContainer.AddComponent<BoxCollider2D>();
                        tag = "Untagged";

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

                        Editor.matrix[j, i, k].tileContainer.tag = tag;

                        // Deaktiviere Player-Dummies & platziere Player

                        if (spriteID >= 600 & spriteID <= 601)
                        {
                            Editor.matrix[j, i, k].tileContainer.SetActive(false);
                            Vector3 position = Editor.matrix[j, i, k].tileContainer.transform.position;
                            players.Add((GameObject)Instantiate(player, new Vector3(position.x, position.y - Editor.tileSize / 2, 0), Editor.matrix[j, i, k].tileContainer.transform.rotation));
                            players[players.Count - 1].name = "Player";

                            if (spriteID == 601)
                            {
                                players[players.Count - 1].GetComponent<SpriteRenderer>().flipX = true;
                            }
                        }
                    }
                }
            }
        }

        // HUD aktivieren

        hud.SetActive(true);

        // HUD & Timer setzen

        if (levelTime > 0)
        {
            remainingTime = levelTime;
            timer = Time.time;
            HUDUpdatePrecount(true);
            HUDUpdateTime(true);
        }
        else
        {
            HUDUpdatePrecount(false);
            HUDUpdateTime(false);
        }

        // Kamera-Zoom auf Standardwert setzen & Raster ausblenden

        camera.orthographicSize = cameraSizeDefault;
        Editor.grid.GetComponent<SpriteRenderer>().enabled = false;
    }

    void GetGameAction()
    {
        int itemID = GetTileFromMatrix(matrixIndex.x, matrixIndex.y, -1);

        if (itemID > 0)
        {
            if (itemID == 224 | itemID == 226)
            {
                GameActionSwitchblock(itemID);
            }

        }
    }

    void GameActionSwitchblock(int itemID)
    {
        if (!actionState)
        {
            actionState = true;
            switchblockTargetState = (224 + 226) - itemID;
            scrollLock = true;
        }

        if (itemID != switchblockTargetState)
        {
            PutTileToMatrix(switchblockTargetState, matrixIndex.x, matrixIndex.y, 0);

            if (switchblockTargetState == 226)
            {
                Editor.matrix[matrixIndex.x, matrixIndex.y, 0].tileContainer.tag = "Wall";
            }
            else
            {
                Editor.matrix[matrixIndex.x, matrixIndex.y, 0].tileContainer.tag = "Untagged";
            }
        }
    }

    bool GetMatrixPosition(Vector3 pointerPosition)
    {
        bool changed = false;
        Vector3 pointerWorldPosition = camera.ScreenToWorldPoint(pointerPosition);

        matrixIndex.xTarget = (int)Mathf.Floor((pointerWorldPosition.x - Editor.matrixOffset.x) / tileSize);
        matrixIndex.yTarget = (int)Mathf.Floor(-(pointerWorldPosition.y - Editor.matrixOffset.y) / tileSize);

        matrixIndex.xTarget = Mathf.Clamp(matrixIndex.xTarget, 0, matrixWidth);
        matrixIndex.yTarget = Mathf.Clamp(matrixIndex.yTarget, 0, matrixHeight);

        if (matrixIndex.x == -1 & matrixIndex.y == -1)
        {
            matrixIndex.x = matrixIndex.xTarget;
            matrixIndex.y = matrixIndex.yTarget;
            //itemDragged = false;
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
                //itemDragged = true;
                return true;
            }
        }

        return false;
    }

    int GetTileFromMatrix(int x, int y, int layer)
    {
        if (x >= 0 & x < matrixWidth & y >= 0 & y < matrixHeight)
        {
            if (layer != -1)
            {
                return Editor.matrix[x, y, layer].spriteID;
            }
            else
            {
                if (Editor.matrix[x, y, 1].spriteID != 0)
                {
                    return Editor.matrix[x, y, 1].spriteID;
                }
                else
                {
                    return Editor.matrix[x, y, 0].spriteID;
                }
            }
        }
        else
        {
            return 0;
        }
    }
        
    void Playing()
    {
        bool matrixPositionChanged;

        if (Input.GetMouseButton(0))
        {
//            if (!actionState)
//            {
//                actionState = true;
//            }
//            else
//            {
                Vector3 pointerScreenPosition2 = Input.mousePosition;
                matrixPositionChanged = GetMatrixPosition(pointerScreenPosition2);

                if (matrixPositionChanged)
                {
                    GetGameAction();
                }
//            }
        }
        else
        {
            if (actionState)
            {
                actionState = false;
                scrollLock = false;
                //drawMode = false;
                //moveMode = false;

                //                if (!itemDragged & editMode != 2)
                //                {
                //                    itemEditing = true;
                //                    GetItemBuildFunction();
                //                }

            }
            matrixIndex.x = -1;
            matrixIndex.y = -1;
        }
    }

    void PutTileToMatrix(int itemSpriteID, int x, int y, int layer)
    {
        if (x >= 0 & x < matrixWidth & y >= 0 & y < matrixHeight)
        {
            if (Editor.matrix[x, y, layer].spriteID != itemSpriteID)
            {
                Editor.matrix[x, y, layer].spriteID = itemSpriteID;

                if (itemSpriteID != 0)
                {
                    Editor.matrix[x, y, layer].tileContainer.GetComponent<SpriteRenderer>().sprite = Editor.sprite[Editor.spriteIDs[itemSpriteID]];
                }
                else
                {
                    Editor.matrix[x, y, layer].tileContainer.GetComponent<SpriteRenderer>().sprite = null;
                }
            }
        }
    }
        
    void Scrolling()
    {
        // Mit Pointereingaben scrollen

        if (Input.GetMouseButton(0) & !scrollLock)
        {              
            if (!pointerState)
            {
                pointerState = true;
                pointerScreenPosition = Input.mousePosition;
                Vector3 pointerWorldPositionA = camera.ScreenToWorldPoint(new Vector3(0, 0, 0));
                Vector3 pointerWorldPositionB = camera.ScreenToWorldPoint(new Vector3(1, 0, 0));
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
    }

	// Use this for initialization

	void Start ()
    {
        InitiateVariables();
        Assignments();
	}

    void TimeControl()
    {
        if (levelTime > 0)
        {
            if (timer < Time.time - 1)
            {
                timer = Time.time;

                if (startMode > 0)
                {
                    startMode--;
                    HUDUpdatePrecount(true);
                }
                else
                {
                    if (remainingTime > 0)
                    {
                        remainingTime--;
                        HUDUpdateTime(true);
                    }
                }
            }
        }
    }
	
	// Update is called once per frame

	void Update ()
    {
        if (General.programState > 0)
        {
            Playing();
            Scrolling();
            TimeControl();
        }
    }
}
