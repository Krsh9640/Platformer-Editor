using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ItemRepositoryControl : MonoBehaviour {

    public void CloseItemRepository()
    {
        gameObject.SetActive(false);
    }

    public void OpenItemRepository()
    {
        if (Editor.editMode == 1)
        {
            gameObject.SetActive(true);
        }
    }

    public void SetCurrentItem(Image item)
    {
        //Debug.Log(item.name);
        SetCurrentItemID(item.name);
    }

    public void SetCurrentItemID(string name)
    {
        switch (name)
        {
            case "IRItemConveyor":
                
                if (Editor.currentItemID != 270)
                {
                    Editor.variationItemID = 282;
                }

                Editor.currentItemID = 270;
                break;

            case "IRItemConveyorSwitch":
                Editor.currentItemID = 284;
                break;
                
            case "IRItemDoor":
                Editor.currentItemID = 396;
                break;

            case "IRItemFloor":
                Editor.currentItemID = 100;
                break;

            case "IRItemLadder":
                Editor.currentItemID = 232;
                break;

            case "IRItemPipe":
                Editor.currentItemID = 250;
                break;

            case "IRItemPlayer":
                Editor.currentItemID = 600;
                break;

            case "IRItemSlope":
                Editor.currentItemID = 200;
                break;

            case "IRItemSpikes":
                Editor.currentItemID = 236;
                break;

            case "IRItemSpringA":
                Editor.currentItemID = 238;
                break;

            case "IRItemSpringB":
                Editor.currentItemID = 240;
                break;

            case "IRItemSteelbeam":
                Editor.currentItemID = 228;
                break;

            case "IRItemSwitchfloor0":
                Editor.currentItemID = 224;
                break;

            case "IRItemSwitchfloor1":
                Editor.currentItemID = 226;
                break;
        }
    }

	// Use this for initialization

	void Start ()
    {
	
	}
	
	// Update is called once per frame

	void Update ()
    {
	
	}
}