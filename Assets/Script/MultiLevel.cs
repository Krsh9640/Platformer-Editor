using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MultiLevel : MonoBehaviour
{
    public GameObject MultiLevelPnl;
    private GameObject DownloadSceneManager, Manager;

    private SaveHandler saveHandler;
    private TilesCreator tilesCreator;

    public TMP_Text levelVal;

    public GameObject CreateLevel2or3Pnl;

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
        if(saveHandler.level2isCreated == true)
        {
            StartCoroutine(LoadLevelOrder(level2filename));
        }
        else
        {
            levelVal.text = "Level 2";
            CreateLevel2or3Pnl.SetActive(true);
        }
    }

    public void Level3Load()
    {
        if(saveHandler.level3isCreated == true)
        {
            StartCoroutine(LoadLevelOrder(level3filename));
        }
        else
        {          
            levelVal.text = "Level 3";
            CreateLevel2or3Pnl.SetActive(true);
        }
    }

    public IEnumerator LoadLevelOrder(string currentLevelName)
    {
        saveHandler.OnSave();

        saveHandler.filename = currentLevelName;

        tilesCreator.ClearTiles();

        saveHandler.OnLoad();

        yield return null;
    }

    public void CreateLevel()
    {
        if(levelVal.text == "Level 2")
        {
            saveHandler.Createjson(level2filename);
            saveHandler.level2isCreated = true;
        }else if(levelVal.text == "Level 3")
        {
            saveHandler.Createjson(level3filename);
            saveHandler.level3isCreated = true;
        }
    }

    public void LoadLevel()
    {
        if (levelVal.text == "Level 2")
        {
            saveHandler.filename = level2filename;
            tilesCreator.ClearTiles();
            saveHandler.OnLoad();
            saveHandler.level2isCreated = true;
        }
        else if (levelVal.text == "Level 3")
        {
            saveHandler.filename = level3filename;
            tilesCreator.ClearTiles();
            saveHandler.OnLoad();
            saveHandler.level3isCreated = true;
        }
    }
}
