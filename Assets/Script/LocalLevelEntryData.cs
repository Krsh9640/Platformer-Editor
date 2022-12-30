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
    public TMP_Text creatorText;
    public Image levelIcon;

    private SaveHandler saveHandler;
    private LevelManager levelManager;

    private void Start()
    {
        transform.position = Vector3.zero;
        transform.localScale = Vector3.one;

        nameText.text = levelName;
        levelIcon.sprite = levelIcon.sprite;
        creatorText.text = creatorName;

        saveHandler = GameObject.Find("DownloadSceneManager").GetComponent<SaveHandler>();
    }

    public void LoadLevel()
    {
        GameObject.Find("DownloadSceneManager").GetComponent<DownloadScene>().LoadLevel();

        saveHandler.levelName = levelName;
        saveHandler.SaveScoreToDefault();
    }
}
