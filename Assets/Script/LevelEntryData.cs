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
    [System.NonSerialized] public string levelName, creatorName, bestPlayerName, bestTimeFormat;
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
        creatorText.text = creatorName;

        Debug.Log(creatorName);

        saveHandler = GameObject.Find("DownloadSceneManager").GetComponent<SaveHandler>();

        if(saveHandler.levelName == levelName)
        {
            bestPlayerNameText.text = bestPlayerName;
            bestPlayerTimeText.text = bestTimeFormat;
            bestPlayerCoinText.text = bestCoin.ToString();
        }
    }

    public void DownloadLevel()
    {
        saveHandler.levelName = levelName;
        GameObject.Find("DownloadSceneManager").GetComponent<DownloadScene>().LoadLevel();
    }

    public void PlayLevel()
    {
        saveHandler.levelName = levelName;
        GameObject.Find("DownloadSceneManager").GetComponent<DownloadScene>().PlayLevel();
    }
}
