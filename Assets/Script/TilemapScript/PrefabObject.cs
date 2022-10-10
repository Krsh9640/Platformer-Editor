using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "PrefabBuildable", menuName = "PrefabObjects/Create Buildable")]
public class PrefabObject : ScriptableObject
{
    [SerializeField] TileCategory category;
    [SerializeField] GameObject gameObject;
    [SerializeField] PlaceType placeType;

    public GameObject GameObject
    {
        get
        {
            return gameObject;
        }
    }

    public PlaceType PlaceType
    {
        get
        {
            return placeType == PlaceType.None ? category.PlaceType : placeType;
        }
    }

    public TileCategory Category
    {
        get
        {
            return category;
        }
    }

}
