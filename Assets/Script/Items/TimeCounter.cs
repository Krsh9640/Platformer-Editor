using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeCounter : MonoBehaviour
{
    public static float currentTime = 0;
    public static int timeSeconds;

    public TMP_Text timeText;

    public GameState gameState;

    void Update()
    {
        gameState.GetComponent<GameState>();
        
        if(gameState.isPlay == true){
            currentTime += Time.deltaTime;
            timeSeconds = (int)(currentTime);
            
            if(timeText.text != timeSeconds.ToString()){
                timeText.text = timeSeconds.ToString();
            } 
        } else if(gameState.isPlay == false){
            timeSeconds = 0;
        }
    }
}
