using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using LootLocker.Requests;

public class LevelEntryData : MonoBehaviour
{
    [System.NonSerialized] public int iD, bestCoin;
    [System.NonSerialized] public string levelName, bestTimeFormat;
    public TMP_Text nameText;
    public TMP_Text creatorText;
    public Image levelIcon;

    public TMP_Text bestPlayerNameText, bestPlayerTimeText, bestPlayerCoinText;

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

    public void DownloadLevel()
    {
        saveHandler.levelNameOnly = levelName;
        saveHandler.levelName = levelName;
        GameObject.Find("DownloadSceneManager").GetComponent<DownloadScene>().LoadLevel();
    }

    public void PlayLevel()
    {
        saveHandler.levelNameOnly = levelName;
        saveHandler.levelName = levelName;
        GameObject.Find("DownloadSceneManager").GetComponent<DownloadScene>().PlayLevel();
    }
}
