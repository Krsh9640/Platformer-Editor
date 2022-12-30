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

    private LoadingScreen loadingScreen;

    private SaveHandler saveHandler;

    private void Awake()
    {
        saveHandler = GameObject.Find("DownloadSceneManager").GetComponent<SaveHandler>();
        loadingScreen = GameObject.Find("DownloadSceneManager").GetComponent<LoadingScreen>();
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

    public void HomeButton(){
        loadingScreen.LoadScene("Loading Screen", "Main Menu");
    }
}
