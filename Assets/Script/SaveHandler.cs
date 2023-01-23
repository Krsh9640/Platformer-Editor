using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

using TMPro;
using UnityEngine.SceneManagement;

public class SaveHandler : Singleton<SaveHandler>
{
    private Dictionary<string, Tilemap> tilemaps = new Dictionary<string, Tilemap>();
    private Dictionary<Tile, TilesObject> tileToTilesObject = new Dictionary<Tile, TilesObject>();
    private Dictionary<String, Tile> guidTotile = new Dictionary<string, Tile>();

    [SerializeField] private BoundsInt bounds;
    public string filename, levelName, levelNameOnly;

    [SerializeField]
    private string level1Filename = "TilemapDataLevel1.json",
        level2Filename = "TilemapDataLevel2.json",
        level3Filename = "TilemapDataLevel3.json";
    public TMP_Text level1Text, level2Text, level3Text;

    public bool level2isCreated = false, level3isCreated = false, moveLevelComplete = false;

    [System.NonSerialized] public string bestPlayerName, bestTimeFormat;

    [System.NonSerialized] public int bestCoin;

    [System.NonSerialized] public float bestTime;

    public GameObject[] prefabObject;

    [SerializeField] private Grid grid;

    private DownloadScene downloadScene;
    private Vector3 newScale = new Vector3(1, 1, 1);

    public void Createjson(string filename)
    {
        List<TilemapData> data = new List<TilemapData>();

        FileHandler.SaveToJSON<TilemapData>(data, levelName, filename);
    }

    public void Level2Json()
    {
        Createjson(level2Filename);

        level2Text.text = "Level 2";
        level2Text.fontSize = 40;

        level2isCreated = true;
    }

    public void Level3Json()
    {
        Createjson(level3Filename);

        level3Text.text = "Level 3";
        level3Text.fontSize = 30;

        level3isCreated = true;
    }

    private void Update()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (scene.name == "Level Editor")
        {
            grid = GameObject.Find("Grid").GetComponent<Grid>();
            downloadScene = GameObject.Find("DownloadSceneManager").GetComponent<DownloadScene>();
        }
    }

    public void initTileReference()
    {
        TilesObject[] buildables = Resources.LoadAll<TilesObject>("Scriptables/Buildables/");

        foreach (TilesObject buildable in buildables)
        {
            if (!tileToTilesObject.ContainsKey(buildable.Tile))
            {
                tileToTilesObject.Add(buildable.Tile, buildable);
                guidTotile.Add(buildable.name, buildable.Tile);
            }
            else
            {
                Debug.LogError("tile " + buildable.Tile.name + " is already in use by " + tileToTilesObject[buildable.Tile].name);
            }
        }
    }

    public void initTilemaps()
    {
        Tilemap[] maps = FindObjectsOfType<Tilemap>();

        foreach (var map in maps)
        {
            if (map.name != "previewMap")
            {
                if (map.name != "defaultMap")
                {
                    tilemaps.Add(map.name, map);
                }
            }
        }
    }

    public void OnSave()
    {
        List<TilemapData> data = new List<TilemapData>();

        foreach (var mapObj in tilemaps)
        {
            TilemapData mapData = new TilemapData();
            mapData.key = mapObj.Key;

            BoundsInt boundsForThisMap = mapObj.Value.cellBounds;

            for (int x = boundsForThisMap.xMin; x < boundsForThisMap.xMax; x++)
            {
                for (int y = boundsForThisMap.yMin; y < boundsForThisMap.yMax; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    Tile tile = mapObj.Value.GetTile<Tile>(pos);

                    prefabObject = FindGameObjectsWithLayer(8);

                    if (tile != null && tileToTilesObject.ContainsKey(tile))
                    {
                        string guid = tileToTilesObject[tile].name;

                        if (prefabObject != null)
                        {
                            foreach (GameObject go in prefabObject)
                            {
                                Vector3 cellPos = grid.WorldToCell(go.transform.position);
                                Vector3 scale = go.transform.localScale;

                                if (go.name == guid)
                                {
                                    if (cellPos == pos)
                                    {
                                        newScale = scale;
                                        Debug.Log(newScale);
                                    }
                                }
                            }
                        }

                        TileInfo ti = new TileInfo(pos, newScale, guid);
                        mapData.tiles.Add(ti);
                    }
                }
            }
            data.Add(mapData);
        }

        FileHandler.SaveToJSON<TilemapData>(data, levelName, filename);
    }



    public void MoveFilesCheck()
    {
        if (downloadScene.fromLocal == true)
        {
            string originalPath = Application.persistentDataPath + "/" + levelNameOnly;
            Debug.Log("FromLocal: " + originalPath);
            MoveFiles(originalPath);
        }
        else if(downloadScene.fromEditor == true)
        {
            string originalPath = Application.persistentDataPath;
            Debug.Log("FromEditor: " + originalPath);
            MoveFiles(originalPath);
        }
    }

    public void MoveFiles(string path)
    {
        FileInfo[] getOriginalFiles = new DirectoryInfo(Application.persistentDataPath).GetFiles("*.*");
        string filepath = Path.Combine(path, levelName);
        Debug.Log(filepath);

        if (!Directory.Exists(filepath))
        {
            Directory.CreateDirectory(filepath);
        }

        foreach (FileInfo file in getOriginalFiles)
        {
            string newFileName = file.Name;
            string destFile = filepath + "/" + newFileName;

            if (!File.Exists(destFile) && file.Extension != ".log")
            {
                File.Move(file.FullName, destFile);

                if (File.Exists(destFile))
                {
                    moveLevelComplete = true;
                }
            }
        }
    }

    public void OnLoad()
    {
        List<TilemapData> data = FileHandler.ReadListFromJSON<TilemapData>(levelName, filename);
        Debug.Log(Path.Combine(levelName, filename));

        foreach (var mapData in data)
        {
            if (!tilemaps.ContainsKey(mapData.key))
            {
                Debug.LogError("Found saved data for tilemap called " + mapData.key + ", but tilemaps does not exist. Skip.");
                continue;
            }
            var map = tilemaps[mapData.key];

            map.ClearAllTiles();

            if (mapData.tiles != null && mapData.tiles.Count > 0)
            {
                foreach (var tile in mapData.tiles)
                {
                    if (guidTotile.ContainsKey(tile.guidForBuildable))
                    {
                        map.SetTile(tile.position, guidTotile[tile.guidForBuildable]);

                        prefabObject = FindGameObjectsWithLayer(8);

                        if (prefabObject != null)
                        {
                            foreach (GameObject go in prefabObject)
                            {
                                go.name = go.name.Replace("(Clone)", "").Trim();
                                Vector3 cellPos = grid.WorldToCell(go.transform.position);

                                if (go.name == guidTotile[tile.guidForBuildable].name)
                                {
                                    if (cellPos == tile.position)
                                    {
                                        go.transform.localScale = tile.localScale;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Reference " + tile.guidForBuildable + " could not be found");
                    }
                }
            }
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

    public void SaveScore()
    {
        ScoreData scoreData = new ScoreData();

        scoreData.levelName = levelName;
        scoreData.playerName = PlayerPrefs.GetString("PlayerID");
        scoreData.bestCoin = PlayerPrefs.GetInt("Coin");
        scoreData.bestTime = PlayerPrefs.GetFloat("Time");
        scoreData.bestTimeFormat = PlayerPrefs.GetString("TimeFormat");

        string json = JsonUtility.ToJson(scoreData, true);
        File.WriteAllText(Application.persistentDataPath + "/" + levelName + "/ScoreData.json", json);
    }

    public void LoadScore()
    {
        string json = File.ReadAllText(Application.persistentDataPath + "/Downloaded/" + levelName + "/ScoreData.json");
        ScoreData scoreData = JsonUtility.FromJson<ScoreData>(json);

        levelName = scoreData.levelName;
        bestPlayerName = scoreData.playerName;
        bestCoin = scoreData.bestCoin;
        bestTime = scoreData.bestTime;
        bestTimeFormat = scoreData.bestTimeFormat;
    }

    public void SaveScoreToDefault()
    {
        ScoreData scoreData = new ScoreData();

        scoreData.levelName = levelName;
        bestPlayerName = "";
        bestCoin = 0;
        bestTime = 0;
        bestTimeFormat = "00:00";

        string json = JsonUtility.ToJson(scoreData, true);
        File.WriteAllText(Application.persistentDataPath + "/" + levelName + "/ScoreData.json", json);
    }

    public void CompareScore()
    {
        LoadScore();

        if (PlayerPrefs.GetInt("Time") < bestTime || bestTime == 0)
        {
            if (PlayerPrefs.GetInt("Coin") > bestCoin || bestCoin == 0)
            {
                SaveScore();
            }
        }
    }
}

[Serializable]
public class TilemapData
{
    public string key;
    public List<TileInfo> tiles = new List<TileInfo>();
}

[Serializable]
public class TileInfo
{
    public string guidForBuildable;
    public Vector3Int position;
    public Vector3 localScale;

    public TileInfo(Vector3Int pos, Vector3 scale, string guid)
    {
        position = pos;
        localScale = scale;
        guidForBuildable = guid;
    }
}

[Serializable]
public class ScoreData
{
    public string levelName;
    public string playerName = "-";
    public int bestCoin = 0;
    public float bestTime = 0;
    public string bestTimeFormat = "00:00";
}
