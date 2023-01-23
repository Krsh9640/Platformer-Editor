using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TimeCounter : MonoBehaviour
{
    public static float currentTime;
    public float currentTimeCompare;
    public static string currentTimeText;

    public bool timerGoing;

    public TMP_Text timeText;

    public GameState gameState;

    private void Update()
    {
        gameState.GetComponent<GameState>();

        if (gameState.isPlay == true)
        {
            BeginTimer();
        }
        else
        {
            EndTimer();
        }

        Debug.Log(currentTimeCompare);
    }

    public void BeginTimer()
    {
        timerGoing = true;
        currentTime = 0f;

        StartCoroutine(UpdateTimer());
    }

    public void EndTimer()
    {
        timerGoing = false;
        currentTime = 0f;
    }

    public IEnumerator UpdateTimer()
    {
        while (timerGoing == true)
        {
            currentTimeCompare += Time.smoothDeltaTime;
            currentTime += Time.smoothDeltaTime;

            if (currentTime < 0)
            {
                currentTime = 0;
            }
            else if (currentTime > 0)
            {
                currentTime += 1;
            }

            float minutes = Mathf.FloorToInt(currentTime / 60);
            float seconds = Mathf.FloorToInt(currentTime % 60);

            currentTimeText = string.Format("{0:00}:{1:00}", minutes, seconds);

            timeText.text = currentTimeText;

            yield return null;
        }
    }
}