using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.Linq;
using TMPro;

public class SaveHandler : Singleton<SaveHandler>
{
    private Dictionary<string, Tilemap> tilemaps = new Dictionary<string, Tilemap>();
    private Dictionary<TileBase, TilesObject> tileBaseToTilesObject = new Dictionary<TileBase, TilesObject>();
    private Dictionary<String, TileBase> guidToTileBase = new Dictionary<string, TileBase>();

    [SerializeField] private BoundsInt bounds;
    [System.NonSerialized] public string filename, levelName;

    [SerializeField]
    private string level1Filename = "TilemapDataLevel1.json",
        level2Filename = "TilemapDataLevel2.json",
        level3Filename = "TilemapDataLevel3.json";
    public TMP_Text level1Text, level2Text, level3Text;

    [System.NonSerialized] public bool level2isCreated, level3isCreated;

    [System.NonSerialized] public string bestPlayerName, bestTimeFormat;
    
    [System.NonSerialized] public int bestCoin, bestTime;

    public void Createjson(string filename)
    {
        List<TilemapData> data = new List<TilemapData>();

        FileHandler.SaveToJSON<TilemapData>(data, levelName, filename);
    }

    public void Level2Json()
    {
        Createjson(level2Filename);

        level2Text.text = "Level 2";
        level2Text.fontSize = 35;

        level2isCreated = true;
    }

    public void Level3Json()
    {
        Createjson(level3Filename);

        level3Text.text = "Level 3";
        level3Text.fontSize = 35;

        level3isCreated = true;
    }

    public void initTileReference()
    {
        TilesObject[] buildables = Resources.LoadAll<TilesObject>("Scriptables/Buildables/");

        foreach (TilesObject buildable in buildables)
        {
            if (!tileBaseToTilesObject.ContainsKey(buildable.TileBase))
            {
                tileBaseToTilesObject.Add(buildable.TileBase, buildable);
                guidToTileBase.Add(buildable.name, buildable.TileBase);
            }
            else
            {
                Debug.LogError("TileBase " + buildable.TileBase.name + " is already in use by " + tileBaseToTilesObject[buildable.TileBase].name);
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
                    TileBase tile = mapObj.Value.GetTile(pos);

                    if (tile != null && tileBaseToTilesObject.ContainsKey(tile))
                    {
                        string guid = tileBaseToTilesObject[tile].name;
                        TileInfo ti = new TileInfo(pos, guid);
                        mapData.tiles.Add(ti);
                    }
                }
            }

            data.Add(mapData);
        }

        FileHandler.SaveToJSON<TilemapData>(data, levelName, filename);
    }

    public void MoveFiles()
    {
        string originalPath = Application.persistentDataPath;
        FileInfo[] getOriginalFiles = new DirectoryInfo(Application.persistentDataPath).GetFiles("*.*");
        string filepath = Path.Combine(originalPath, levelName);
        Debug.Log(filepath);

        if (!Directory.Exists(filepath))
        {
            Directory.CreateDirectory(filepath);
        }

        foreach (FileInfo file in getOriginalFiles)
        {
            string newFileName = file.Name;
            string destFile = Path.Combine(filepath, newFileName);

            if(!File.Exists(destFile)){
                File.Move(file.FullName, destFile);
            }
        }
    }

    public void OnLoad()
    {
        Debug.Log(levelName + " " + filename);
        List<TilemapData> data = FileHandler.ReadListFromJSON<TilemapData>(levelName, filename);

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
                    if (guidToTileBase.ContainsKey(tile.guidForBuildable))
                    {
                        map.SetTile(tile.position, guidToTileBase[tile.guidForBuildable]);
                    }
                    else
                    {
                        Debug.LogError("Reference " + tile.guidForBuildable + " could not be found");
                    }
                }
            }
        }
    }

    public void SaveScore()
    {
        ScoreData scoreData = new ScoreData();

        scoreData.levelName = levelName;
        scoreData.playerName = PlayerPrefs.GetString("PlayerID");
        scoreData.bestCoin = PlayerPrefs.GetInt("Coin");
        scoreData.bestTime = PlayerPrefs.GetInt("Time");
        scoreData.bestTimeFormat = PlayerPrefs.GetString("TimeFormat");

        string json = JsonUtility.ToJson(scoreData, true);
        File.WriteAllText(Application.persistentDataPath + "/" + levelName + "/ScoreData.json", json);
    }

    public void LoadScore()
    {
        string json = File.ReadAllText(Application.persistentDataPath + "/" + levelName + "/ScoreData.json");
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

    public TileInfo(Vector3Int pos, string guid)
    {
        position = pos;
        guidForBuildable = guid;
    }
}

[Serializable]
public class ScoreData
{
    public string levelName;
    public string playerName = "-";
    public int bestCoin = 0;
    public int bestTime = 0;
    public string bestTimeFormat = "00:00";
}
