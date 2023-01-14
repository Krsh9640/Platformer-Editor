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
    TilesCreator tilesCreator;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonClicked);
        tilesCreator = TilesCreator.GetInstance();
    }

    private void Update()
    {

    }

    private void ButtonClicked()
    {
        tilesCreator.TileSelected(item);

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

    private IEnumerator PointerExitRoutine(){
        yield return new WaitForSeconds(0.2f);

        Panel.SetActive(false);
    }
}
