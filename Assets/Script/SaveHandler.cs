using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SaveHandler : Singleton<SaveHandler>
{
    private Dictionary<string, Tilemap> tilemaps = new Dictionary<string, Tilemap>();
    private Dictionary<TileBase, TilesObject> tileBaseToTilesObject = new Dictionary<TileBase, TilesObject>();
    private Dictionary<String, TileBase> guidToTileBase = new Dictionary<string, TileBase>();

    private Dictionary<GameObject, TilesObject> PrefabstoTilesObject = new Dictionary<GameObject, TilesObject>();
    private Dictionary<String, GameObject> guidToPrefabObject = new Dictionary<string, GameObject>();

    [SerializeField] private BoundsInt bounds;
    [SerializeField] private string filename = "tilemapData.json";

    private void Start()
    {
        initTilemaps();
        initTileReference();
    }

    private void initTileReference()
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

    private void initTilemaps()
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
    public List<PrefabInfo> prefab = new List<PrefabInfo>();
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
public class PrefabInfo
{
    public string guidForPrefabs;
    public Vector3 position;

    public PrefabInfo(Vector3 pos, string guid)
    {
        position = pos;
        guidForPrefabs = guid;
    }
}