using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TilesButtonHandler : MonoBehaviour
{
    [SerializeField] TilesObject item;
    Button button;

    TilesCreator tilesCreator;

    private void Awake(){
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonClicked);
        tilesCreator = TilesCreator.GetInstance();
    }

    private void ButtonClicked(){
        Debug.Log ("Button was clicked: " + item.name);
        tilesCreator.TileSelected(item);
    }
}
