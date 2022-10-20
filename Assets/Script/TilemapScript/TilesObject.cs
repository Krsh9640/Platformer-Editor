using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Category{
    Terrains,
    Items,
    Enemies,
    Extras
}

[CreateAssetMenu(fileName = "TileBuildable", menuName = "TilesObjects/Create Buildable")]
public class TilesObject : ScriptableObject
{
    [SerializeField] TileCategory category;
    [SerializeField] TileBase tileBase;
    //[SerializeField] GameObject gameObject;
    [SerializeField] PlaceType placeType;

    public TileBase TileBase{
        get{
            return tileBase;
        }
    }

    //public GameObject GameObject{
    //    get
    //    {
    //        return gameObject;
    //    }
    //}

    public PlaceType PlaceType{
        get{
            return placeType == PlaceType.None ? category.PlaceType : placeType;
        }
    }
     
    public TileCategory Category{
        get{
            return category;
        }
    }

}
