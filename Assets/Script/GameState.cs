using UnityEngine;

public class GameState : MonoBehaviour
{
    public GameObject character;
    public GameObject tileTabs;
    public GameObject coinCounter, timeCounter, healthCounter, keyIcon, potionIcon;
    public GameObject stopButton, pauseButton, sidebar, pausePnl;

    private CharacterController2D controller;
    private PlayerMovement movement;
    private Animator playerAnimator;
    private SaveHandler saveHandler;
    private GameObject DownloadSceneManager;

    private GameObject[] enemies;

    private bool isPaused = false;
    public bool isPlay = false, FromEditMode, FromPlayMode;

    public GameObject particle1, particle2;

    private void Start()
    {
        controller = character.GetComponent<CharacterController2D>();
        movement = character.GetComponent<PlayerMovement>();
        playerAnimator = character.GetComponent<Animator>();

        DownloadSceneManager = GameObject.Find("DownloadSceneManager");
        saveHandler = DownloadSceneManager.GetComponent<SaveHandler>();

        if (isPlay == false)
        {
            if (particle1 != null && particle2 != null)
            {
                particle1.GetComponent<ParticleSystem>().Stop();
                particle2.GetComponent<ParticleSystem>().Stop();
            }
        }

        saveHandler.initTilemaps();
        saveHandler.initTileReference();
    }

    private void FixedUpdate()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    private void LateUpdate()
    {
        if (movement.currentHealth <= 0)
        {
            StopPlaying();
            movement.currentHealth = 3;
        }
    }

    public void BtnPlay()
    {
        isPlay = true;
        FromEditMode = true;

        GetComponentEnemy(true);

        controller.enabled = true;
        movement.enabled = true;
        playerAnimator.enabled = true;

        tileTabs.SetActive(false);
        coinCounter.SetActive(true);
        timeCounter.SetActive(true);
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

        GetComponentEnemy(true);

        controller.enabled = true;
        movement.enabled = true;
        playerAnimator.enabled = true;

        tileTabs.SetActive(false);
        coinCounter.SetActive(true);
        timeCounter.SetActive(true);
        healthCounter.SetActive(true);
        pauseButton.SetActive(true);
        sidebar.SetActive(false);

        saveHandler.OnSave();
    }

    public void Restart()
    {
        Coin.totalCoin = 0;
    }

    public void PauseGame()
    {
        pauseButton.SetActive(isPaused);
        pausePnl.SetActive(!isPaused);

        Time.timeScale = 0f;
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void StopPlaying()
    {
        isPlay = false;

        GetComponentEnemy(false);

        controller.enabled = false;
        movement.enabled = false;
        playerAnimator.enabled = false;

        tileTabs.SetActive(true);
        coinCounter.SetActive(false);
        timeCounter.SetActive(false);
        healthCounter.SetActive(false);
        pauseButton.SetActive(false);
        sidebar.SetActive(true);
        stopButton.SetActive(false);

        particle1.GetComponent<ParticleSystem>().Stop();
        particle2.GetComponent<ParticleSystem>().Stop();
    }

    public void GetComponentEnemy(bool isEnabled)
    {
        foreach (GameObject e in enemies)
        {
            if (e != null)
            {
                if (e.TryGetComponent(out MushroomBehaviour mushroom))
                {
                    mushroom.enabled = isEnabled;
                    e.GetComponent<Animator>().enabled = isEnabled;
                    if (isEnabled == true)
                    {
                        e.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                    }
                    else
                    {
                        e.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                    }
                }
                else if (e.TryGetComponent(out FrogBehaviour frog))
                {
                    frog.enabled = isEnabled;
                    e.GetComponent<Animator>().enabled = isEnabled;
                    if (isEnabled == true)
                    {
                        e.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                    }
                    else
                    {
                        e.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                    }
                }
                else if (e.TryGetComponent(out Animator animator))
                {
                    animator.enabled = isEnabled;
                }
            }
        }
    }
}