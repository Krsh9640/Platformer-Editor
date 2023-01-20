using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;

public class TilesButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TilesObject item;
    Button button;
    public GameObject Panel;
    public GameObject needLevel2or3Panel;
    TilesCreator tilesCreator;

    private SaveHandler saveHandler;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonClicked);
        tilesCreator = TilesCreator.GetInstance();
        saveHandler = GameObject.Find("DownloadSceneManager").GetComponent<SaveHandler>();
    }

    private void ButtonClicked()
    {
        tilesCreator.TileSelected(item);

        if (this.gameObject.name == "Tile_LevelWarp")
        {
            if (saveHandler.level2isCreated == false && saveHandler.filename == "TilemapDataLevel1.json")
            {
                tilesCreator.TileSelected(null);
                needLevel2or3Panel.SetActive(true);
            }
            else if (saveHandler.level3isCreated == false && saveHandler.filename == "TilemapDataLevel2.json")
            {
                tilesCreator.TileSelected(null);
                needLevel2or3Panel.SetActive(true);
            }
        }


        if (tilesCreator.selectedTile != null)
        {
            if (tilesCreator.selectedTile.name == item.name)
            {
                tilesCreator.FlipTile();
                tilesCreator.isFlipped = !tilesCreator.isFlipped;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerEnter != Panel)
        {
            if (eventData.pointerEnter == this.gameObject)
            {
                Panel.SetActive(true);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(PointerExitRoutine());
    }

    private IEnumerator PointerExitRoutine()
    {
        yield return new WaitForSeconds(0.15f);

        Panel.SetActive(false);
    }
}
