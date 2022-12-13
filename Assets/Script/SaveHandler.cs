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
    public string filename;

    [SerializeField] private string level1Filename = "TilemapDataLevel1.json", 
        level2Filename = "TilemapDataLevel2.json", 
        level3Filename = "TilemapDataLevel3.json";
    public TMP_Text level1Text, level2Text, level3Text;

    public bool level2isCreated, level3isCreated;

    public void Createjson(string filename)
    {
        List<TilemapData> data = new List<TilemapData>();

        FileHandler.SaveToJSON<TilemapData>(data, filename);
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

        FileHandler.SaveToJSON<TilemapData>(data, filename);
    }

    public void OnLoad()
    {
        List<TilemapData> data = FileHandler.ReadListFromJSON<TilemapData>(filename);

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

    //public GameObject[] FindGameObjectWithinLayer(int layer)
    //{
    //    GameObject[] goArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];

    //    List<GameObject> goList = new List<GameObject>();

    //    for (int i = 0; i < goArray.Length; i++)
    //    {
    //        if (goArray[i].layer == layer)
    //        {
    //            goList.Add(goArray[i]);
    //        }
    //    }
    //    if (goList.Count == 0)
    //    {
    //        return null;
    //    }
    //    return goList.ToArray();
    //}

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
