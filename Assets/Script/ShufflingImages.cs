using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShufflingImages : MonoBehaviour
{
    public GameObject[] FillerImages;

    private void Awake()
    {
        ShuffleImage();
    }

    public void ShuffleImage()
    {
        int randomNumber = Random.Range(0, 2);

        switch (randomNumber)
        {
            case 0:
                FillerImages[1].SetActive(true);
                break;
            case 1:
                FillerImages[2].SetActive(true);
                break;
            case 2:
                FillerImages[3].SetActive(true);
                break;
        }
    }
}
