using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using LootLocker.Requests;

public class LocalLevelEntryData : MonoBehaviour
{
    public string levelName, creatorName;
    public TMP_Text nameText;
    public Image levelIcon;

    private SaveHandler saveHandler;
    private LevelManager levelManager;

    private void Start()
    {
        transform.position = Vector3.zero;
        transform.localScale = Vector3.one;

        nameText.text = levelName;
        levelIcon.sprite = levelIcon.sprite;

        saveHandler = GameObject.Find("DownloadSceneManager").GetComponent<SaveHandler>();
    }

    public void LoadLevel()
    {
        saveHandler.levelNameOnly = levelName;
        saveHandler.levelName = levelName;
        GameObject.Find("DownloadSceneManager").GetComponent<DownloadScene>().LocalLoadLevel();
    }
}
