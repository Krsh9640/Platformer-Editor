using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

public class MainMenu : MonoBehaviour
{
    public GameObject BtnsMainMenu;
    public GameObject PnlLevels;

    [SerializeField] private LoadingScreen loadingScreen;

    [SerializeField] private SaveHandler saveHandler;

    private DownloadScene downloadScene;

    private void Update()
    {
        if (saveHandler == null)
        {
            saveHandler = GameObject.Find("DownloadSceneManager").GetComponent<SaveHandler>();
        }

        if (loadingScreen == null)
        {
            loadingScreen = GameObject.Find("DownloadSceneManager").GetComponent<LoadingScreen>();
        }

        if (downloadScene == null)
        {
            downloadScene = GameObject.Find("DownloadSceneManager").GetComponent<DownloadScene>();
        }
    }

    //Exit Button//
    public void Exit()
    {
        Application.Quit();
    }

    //Create a Level Button//
    public void BtnCreateaLevel()
    {
        BtnsMainMenu.SetActive(false);
        PnlLevels.SetActive(true);
    }


    //Make a Level Button, go to Level Editor//
    public void MakeaLevel()
    {
        saveHandler.filename = "TilemapDataLevel1.json";

        loadingScreen.LoadScene("Loading Screen", "Level Editor");
    }

    public void HomeButton()
    {
        loadingScreen.LoadScene("Loading Screen", "Main Menu");
        Time.timeScale = 1f;
    }
}
