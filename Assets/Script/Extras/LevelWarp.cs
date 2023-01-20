using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelWarp : MonoBehaviour
{
    private GameObject DownloadSceneManager;
    private SaveHandler saveHandler;

    private GameObject Manager;
    private GameState gameState;
    private TilesCreator tilesCreator;

    public GameObject NextLevel, NextLevelTextObj, PlayerChar;
    public TMP_Text NextLevelText;
    private Image fadeOutUIImage;
    public float fadeSpeed = 0.8f;
    private PlayerMovement playerMovement;
    private TimeCounter timeCounter;

    public BoxCollider2D boxCollider;

    public static int currentCoin;
    public static float currentTime;

    public bool hasLoaded = true;

    private void Awake()
    {
        DownloadSceneManager = GameObject.Find("DownloadSceneManager");
        saveHandler = DownloadSceneManager.GetComponent<SaveHandler>();
        Manager = GameObject.Find("Manager");
        gameState = Manager.GetComponent<GameState>();
        tilesCreator = Manager.GetComponent<TilesCreator>();

        NextLevel = GameObject.Find("NextLevel");
        fadeOutUIImage = NextLevel.GetComponent<Image>();

        NextLevelTextObj = GameObject.Find("NextLevelText");
        NextLevelText = NextLevelTextObj.GetComponent<TMP_Text>();

        PlayerChar = GameObject.FindWithTag("Player");
        playerMovement = PlayerChar.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (saveHandler.filename == "TilemapDataLevel1.json")
        {
            NextLevelText.text = "Level 1";
        }
        else if (saveHandler.filename == "TilemapDataLevel2.json")
        {
            NextLevelText.text = "Level 2";
        }
        else if (saveHandler.filename == "TilemapDataLevel3.json")
        {
            NextLevelText.text = "Level 3";
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            if (NextLevelText.text == "Level 1" && saveHandler.filename == "TilemapDataLevel1.json")
            {
                NextLevelText.text = "Level 2";
                StartCoroutine(MultiLevelLoad(saveHandler.filename, "TilemapDataLevel2.json"));
                hasLoaded = true;
            }
            else if (NextLevelText.text == "Level 2" && saveHandler.filename == "TilemapDataLevel2.json")
            {
                NextLevelText.text = "Level 3";
                StartCoroutine(MultiLevelLoad(saveHandler.filename, "TilemapDataLevel3.json"));
                hasLoaded = true;
            }
        }
        else if (other.gameObject.tag != "Player")
        {
            boxCollider.isTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != "Player")
        {
            boxCollider.isTrigger = false;
        }
    }


    public IEnumerator MultiLevelLoad(string currentFilename, string nextFilename)
    {
        if (gameState.isPlay == true)
        {
            timeCounter = GameObject.Find("TimeCounter").GetComponent<TimeCounter>();

            if (saveHandler.filename == currentFilename)
            {
                playerMovement.rb.velocity = new Vector2(0, 0);
                fadeOutUIImage.enabled = true;
                NextLevelText.enabled = true;

                saveHandler.filename = nextFilename;

                yield return new WaitForSeconds(0.5f);

                PlayerChar.transform.position = playerMovement.startingSpawnPos;

                saveHandler.OnLoad();

                PlayerChar.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

                fadeOutUIImage.enabled = false;
                NextLevelText.enabled = false;

                if (hasLoaded == true && gameState.FromPlayMode == true)
                {
                    currentCoin = Coin.totalCoin;
                    currentTime = timeCounter.currentTimeCompare;

                    PlayerPrefs.SetInt("Coin", currentCoin);
                    PlayerPrefs.SetFloat("Time", currentTime);

                    hasLoaded = false;
                }
            }
        }
        yield return null;
    }
}
