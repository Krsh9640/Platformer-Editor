using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using LootLocker.Requests;

public class LevelEntryData : MonoBehaviour
{
    public int iD;
    public string levelName;
    public string creatorName;
    public TMP_Text nameText;
    public TMP_Text creatorText;
    public Image levelIcon;

    private GameObject BrowseUI;

    private void Start()
    {
        transform.position = Vector3.zero;
        transform.localScale = Vector3.one;

        nameText.text = levelName;
        levelIcon.sprite = levelIcon.sprite;

        PlayerName();

        BrowseUI = GameObject.Find("LevelBrowserPanel");
    }

    public void DownloadLevel()
    {
        GameObject.Find("DownloadSceneManager").GetComponent<DownloadScene>().LoadLevel();
    }

    public void PlayLevel()
    {
        GameObject.Find("DownloadSceneManager").GetComponent<DownloadScene>().PlayLevel();
    }

    public void PlayerName()
    {
        LootLockerSDKManager.GetPlayerName((response) =>
        {
            if (response.success)
            {
                creatorName = response.name;
                creatorText.text = creatorName;
            }
            else
            {

            }
        });
    }
}
