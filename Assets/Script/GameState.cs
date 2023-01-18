using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public GameObject character;
    public GameObject tileTabs;
    public GameObject coinCounter, timeElapsed, healthCounter, keyIcon;
    public GameObject stopButton, pauseButton, sidebar, pausePnl;

    private CharacterController2D controller;
    private PlayerMovement movement;
    private Animator playerAnimator;
    [SerializeField] private SaveHandler saveHandler;
    private TilesCreator tilesCreator;

    [SerializeField] private GameObject Enemies;

    private bool isPaused = false;
    public bool isPlay = false, FromEditMode, FromPlayMode;

    public GameObject particle1, particle2;

    private TimeCounter timeCounter;

    public int currentCoins { get; set; }
    public float currentTimes { get; set; }

    private List<GameObject> go = new List<GameObject>();

    private void Start()
    {
        controller = character.GetComponent<CharacterController2D>();
        movement = character.GetComponent<PlayerMovement>();
        playerAnimator = character.GetComponent<Animator>();
        tilesCreator = GetComponent<TilesCreator>();

        saveHandler = GameObject.Find("DownloadSceneManager").GetComponent<SaveHandler>();

        if (isPlay == false)
        {
            if (particle1 != null && particle2 != null)
            {
                particle1.GetComponent<ParticleSystem>().Stop();
                particle2.GetComponent<ParticleSystem>().Stop();
            }
        }

        timeCounter = timeElapsed.GetComponent<TimeCounter>();

        saveHandler.initTilemaps();
        saveHandler.initTileReference();

        Enemies = GameObject.Find("Enemies");
    }

    private void LateUpdate()
    {
        if (movement.currentHealth <= 0)
        {
            StopPlaying();
            movement.currentHealth = 5;
        }

        if (isPlay == true)
        {
            GetComponentEnemy(true);
        }
        else
        {
            GetComponentEnemy(false);
        }
    }

    public void BtnPlay()
    {
        isPlay = true;
        FromEditMode = true;

        controller.enabled = true;
        movement.enabled = true;
        playerAnimator.enabled = true;

        tileTabs.SetActive(false);
        coinCounter.SetActive(true);
        timeElapsed.SetActive(true);
        healthCounter.SetActive(true);
        pauseButton.SetActive(true);
        stopButton.SetActive(true);
        sidebar.SetActive(false);

        saveHandler.OnSave();
    }

    public void PlayMode()
    {
        isPlay = true;
        FromPlayMode = true;

        controller.enabled = true;
        movement.enabled = true;
        playerAnimator.enabled = true;

        tileTabs.SetActive(false);
        coinCounter.SetActive(true);
        timeElapsed.SetActive(true);
        healthCounter.SetActive(true);
        pauseButton.SetActive(true);
        sidebar.SetActive(false);
    }

    public void StopPlaying()
    {
        isPlay = false;

        controller.enabled = false;
        movement.enabled = false;
        playerAnimator.enabled = false;

        tileTabs.SetActive(true);
        coinCounter.SetActive(false);
        timeElapsed.SetActive(false);
        healthCounter.SetActive(false);
        pauseButton.SetActive(false);
        sidebar.SetActive(true);
        stopButton.SetActive(false);

        particle1.GetComponent<ParticleSystem>().Stop();
        particle2.GetComponent<ParticleSystem>().Stop();

        timeCounter.timerGoing = false;

        character.transform.position = movement.startingSpawnPos;
        saveHandler.OnLoad();
    }

    public void Restart()
    {
        if (UnlockedDoor.isWin == true)
        {
            currentCoins = 0;
            currentTimes = 0;

            tilesCreator.ClearTiles();

            character.transform.position = movement.startingSpawnPos;
            saveHandler.filename = "TilemapDataLevel1.json";
            saveHandler.OnLoad();
            PauseGame();

            vCamTrigger.IsEnabled = false;
            UnlockedDoor.isWin = false;

            controller.enabled = true;
            movement.enabled = true;
            playerAnimator.enabled = true;
        }
        else
        {
            currentCoins = PlayerPrefs.GetInt("Coin");
            currentTimes = PlayerPrefs.GetFloat("Time");

            tilesCreator.ClearTiles();

            character.transform.position = movement.startingSpawnPos;

            saveHandler.OnLoad();
            PauseGame();

            vCamTrigger.IsEnabled = false;

            controller.enabled = true;
            movement.enabled = true;
            playerAnimator.enabled = true;
        }
    }

    public void PauseGame()
    {
        pauseButton.SetActive(isPaused);
        pausePnl.SetActive(!isPaused);

        Time.timeScale = 0f;
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void GetComponentEnemy(bool isEnabled)
    {
        Rigidbody2D[] rbs = Enemies.transform.GetComponentsInChildren<Rigidbody2D>();

        foreach (Rigidbody2D rb in rbs)
        {
            if (rb != null && rb.gameObject.name != "Enemies")
            {
                if (isEnabled == true)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                }
                else
                {
                    rb.bodyType = RigidbodyType2D.Static;
                }
            }
        }

        Animator[] anims = Enemies.transform.GetComponentsInChildren<Animator>();

        if (anims != null)
        {
            foreach (Animator anim in anims)
            {
                if (anim != null)
                {
                    anim.enabled = isEnabled;
                }
            }
        }

        MushroomBehaviour mushroom = Enemies.transform.GetComponentInChildren<MushroomBehaviour>();
        if (mushroom != null)
        {
            mushroom.enabled = isEnabled;
        }

        FrogBehaviour frog = Enemies.transform.GetComponentInChildren<FrogBehaviour>();
        if (frog != null)
        {
            frog.enabled = isEnabled;
        }
    }
}