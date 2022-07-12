using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Linq;
using System.IO;

public class SaveHandler : Singleton<SaveHandler>
{
    Dictionary<string, Tilemap> tilemaps = new Dictionary<string, Tilemap>();
    Dictionary<TileBase, TilesObject> tileBaseToTilesObject = new Dictionary<TileBase, TilesObject>();
    Dictionary<String, TileBase> guidToTileBase = new Dictionary<string, TileBase>();
    [SerializeField] BoundsInt bounds;
    [SerializeField] string filename = "tilemapData.json";

    private void Start() {
        initTilemaps();
        initTileReference();
    }

    private void initTileReference(){
        TilesObject[] buildables = Resources.LoadAll<TilesObject>("Scriptables/Buildables");

        foreach(TilesObject buildable in buildables){
            if(!tileBaseToTilesObject.ContainsKey(buildable.TileBase)){
                tileBaseToTilesObject.Add(buildable.TileBase, buildable);
                guidToTileBase.Add(buildable.name, buildable.TileBase);
            } else{
                Debug.LogError("TileBase " + buildable.TileBase.name + " is already in use by " + tileBaseToTilesObject[buildable.TileBase].name);
            }
        }
    }

    private void initTilemaps(){
        Tilemap[] maps = FindObjectsOfType<Tilemap>();

        foreach(var map in maps) {
            if(map.name != "previewMap"){
                if(map.name != "defaultMap"){
                    tilemaps.Add(map.name, map);
                }
            }
        }
    }

    public void OnSave(){
        List<TilemapData> data = new List<TilemapData>();

        foreach(var mapObj in tilemaps){
            TilemapData mapData = new TilemapData();
            mapData.key = mapObj.Key;

            BoundsInt boundsForThisMap = mapObj.Value.cellBounds;

            for(int x = boundsForThisMap.xMin; x < boundsForThisMap.xMax; x++){
                for(int y = boundsForThisMap.yMin; y < boundsForThisMap.yMax; y++){
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    TileBase tile = mapObj.Value.GetTile(pos);

                    if(tile != null && tileBaseToTilesObject.ContainsKey(tile)){
                        string guid = tileBaseToTilesObject[tile].name;
                        TileInfo ti = new TileInfo(pos, guid);
                        Debug.Log("TileInfo : " + ti);
                        mapData.tiles.Add(ti);
                    }
                }
            }
            data.Add(mapData);
        } 

        FileHandler.SaveToJSON<TilemapData>(data, filename);
    }

    public void OnLoad(){
        List<TilemapData> data = FileHandler.ReadListFromJSON<TilemapData>(filename);

        foreach(var mapData in data){
            if(!tilemaps.ContainsKey(mapData.key)){
                Debug.LogError("Found saved data for tilemap called " + mapData.key + ", but tilemaps does not exist. Skip.");
                continue;
            }
            var map = tilemaps[mapData.key];

            map.ClearAllTiles();

            if(mapData.tiles != null && mapData.tiles.Count > 0){
                foreach(var tile in mapData.tiles){
                    if(guidToTileBase.ContainsKey(tile.guidForBuildable)){
                        map.SetTile(tile.position, guidToTileBase[tile.guidForBuildable]);
                    } else{
                        Debug.LogError("Reference " + tile.guidForBuildable + " could not be found");
                    }
                }
            }
        }
    }
}

[Serializable]
public class TilemapData{
    public string key;
    public List<TileInfo> tiles = new List<TileInfo>();
}

[Serializable]
public class TileInfo{ 
    public string guidForBuildable;
    public Vector3Int position;

    public TileInfo(Vector3Int pos, string guid){
        position = pos;
        guidForBuildable = guid;
    }
}
