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

public enum PlaceType{
    Single,
    Rectangle
}

[CreateAssetMenu(fileName = "Buildable", menuName = "TilesObjects/Create Buildable")]
public class TilesObject : ScriptableObject
{
    [SerializeField] Category category;
    [SerializeField] TileBase tileBase;
    [SerializeField] PlaceType placetype;

    public TileBase TileBase{
        get{
            return tileBase;
        }
    }

    public PlaceType PlaceType{
        get{
            return placetype;
        }
    }
     
    public Category Category{
        get{
            return category;
        }
    }

}
