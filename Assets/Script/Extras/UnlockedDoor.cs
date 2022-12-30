using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnlockedDoor : MonoBehaviour
{
    public GameObject manager, winText, player;
    public GameState gameState;
    public PlayerMovement movement;
    public Animator playerAnimator;

    public GameObject particle1, particle2;

    public static bool isWin = false;
    private GameObject afterWinPnl;

    private SaveHandler saveHandler;
    private LevelManager levelManager;
    [SerializeField] private TimeCounter timeCounter;

    private void Start()
    {
        manager = GameObject.Find("Manager");
        gameState = manager.GetComponent<GameState>();
        winText = GameObject.Find("WinText");
        player = GameObject.FindWithTag("Player");
        movement = player.GetComponent<PlayerMovement>();
        playerAnimator = player.GetComponent<Animator>();

        particle1 = GameObject.Find("ParticleSystem1");
        particle2 = GameObject.Find("ParticleSystem2");

        afterWinPnl = GameObject.Find("AfterWin").transform.GetChild(0).gameObject;

        saveHandler = GameObject.Find("DownloadSceneManager").GetComponent<SaveHandler>();
        levelManager = GameObject.Find("Manager").GetComponent<LevelManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (gameState.isPlay == true)
            {
                StartCoroutine(COWinText());
            }
        }
    }

    public IEnumerator COWinText()
    {
        timeCounter = GameObject.Find("TimeCounter").GetComponent<TimeCounter>();
        if (gameState.FromPlayMode == true)
        {
            isWin = true;

            winText.GetComponent<TMP_Text>().enabled = true;

            particle1.GetComponent<ParticleSystem>().Play();
            particle2.GetComponent<ParticleSystem>().Play();

            playerAnimator.SetFloat("Speed", 0);

            movement.enabled = false;
            movement.rb.velocity = new Vector2(0, 0);
            gameState.GetComponentEnemy(false);
            gameState.isPlay = false;

            timeCounter.timerGoing = false;

            PlayerPrefs.SetInt("Coin", Coin.totalCoin);
            PlayerPrefs.SetInt("Time", timeCounter.currentTimeINTver);
            PlayerPrefs.SetString("TimeFormat", TimeCounter.currentTimeText);

            saveHandler.CompareScore();

            yield return new WaitForSeconds(2);

            levelManager.UpdateScoreHandler();

            yield return new WaitForSeconds(3);

            afterWinPnl.SetActive(true);
        }
        else
        {
            isWin = true;

            winText.GetComponent<TMP_Text>().enabled = true;

            particle1.GetComponent<ParticleSystem>().Play();
            particle2.GetComponent<ParticleSystem>().Play();

            playerAnimator.SetFloat("Speed", 0);

            movement.enabled = false;
            movement.rb.velocity = new Vector2(0, 0);
            gameState.GetComponentEnemy(false);
            gameState.isPlay = false;

            timeCounter.timerGoing = false;

            yield return new WaitForSeconds(1);

            gameState.StopPlaying();

            winText.GetComponent<TMP_Text>().enabled = false;
        }
    }
}
