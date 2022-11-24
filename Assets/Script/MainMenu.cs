using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

public class MainMenu : MonoBehaviour
{
    public GameObject BtnsMainMenu, BtnMakeaLevel, BtnOption;
    public GameObject PnlLevels, PnlOption;

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
        SceneManager.LoadSceneAsync("Level Editor");
    }

    public void HomeButton(){
        SceneManager.LoadSceneAsync("Main Menu");
    }
}
