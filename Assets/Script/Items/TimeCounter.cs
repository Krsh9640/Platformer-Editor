using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeCounter : MonoBehaviour
{
    public static float currentTime = 0;
    private int timeSeconds;

    public TMP_Text timeText;

    public GameState gameState;

    // Update is called once per frame
    void Update()
    {
        if(gameState.isPlay == true){
            currentTime += Time.deltaTime;
            timeSeconds = (int)(currentTime % 60);
            
            if(timeText.text != timeSeconds.ToString()){
                timeText.text = timeSeconds.ToString();
            } else if(gameState.isPlay == false){
                currentTime = 0;
            }
        }
    }
}
