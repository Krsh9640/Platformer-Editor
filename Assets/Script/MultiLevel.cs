using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiLevel : MonoBehaviour
{
    public GameObject MultiLevelPnl;
    private GameObject DownloadSceneManager, Manager;

    private SaveHandler saveHandler;
    private TilesCreator tilesCreator;

    [SerializeField] private string level1filename = "TilemapDataLevel1.json", 
        level2filename = "TilemapDataLevel2.json", 
        level3filename = "TilemapDataLevel3.json";

    private void Awake()
    {
        DownloadSceneManager = GameObject.Find("DownloadSceneManager");
        saveHandler = DownloadSceneManager.GetComponent<SaveHandler>();
        Manager = GameObject.Find("Manager");
        tilesCreator = Manager.GetComponent<TilesCreator>();
    }

    public void MultiLevelPanel()
    {
        if (MultiLevelPnl.activeInHierarchy)
        {
            MultiLevelPnl.SetActive(false);
        }
        else
        {
            MultiLevelPnl.SetActive(true);
        }
    }

    public void Level1Load()
    {
        StartCoroutine(LoadLevelOrder(level1filename));
    }

    public void Level2Load()
    {
        StartCoroutine(LoadLevelOrder(level2filename));
    }

    public void Level3Load()
    {
        StartCoroutine(LoadLevelOrder(level3filename));
    }

    public IEnumerator LoadLevelOrder(string currentLevelName)
    {
        saveHandler.OnSave();

        yield return new WaitForSeconds(1);

        saveHandler.filename = currentLevelName;

        tilesCreator.ClearTiles();

        saveHandler.OnLoad();

        yield return null;
    }
}
