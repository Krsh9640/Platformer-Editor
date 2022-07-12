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

    //Back Button from Create a Level to Main Menu//
    public void BackToMenu()
    {
        PnlLevels.SetActive(false);
        BtnsMainMenu.SetActive(true);
    }

    //Make a Level Button, go to Level Editor//
    public void MakeaLevel()
    {
        SceneManager.LoadSceneAsync("Level Editor");
    }
    
    public void OptionButton(){
        BtnsMainMenu.SetActive(false);
        PnlOption.SetActive(true);
    }

    public void closeOption(){
        PnlOption.SetActive(false);
        BtnsMainMenu.SetActive(true);
    }

    public void HomeButton(){
        SceneManager.LoadSceneAsync("Main Menu");
    }
}
