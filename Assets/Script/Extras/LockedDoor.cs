using System.Collections;
using TMPro;
using UnityEngine;
using LootLocker.Requests;

public class LockedDoor : MonoBehaviour
{
    public GameObject manager, winText, player;
    public GameState gameState;
    public PlayerMovement movement;
    public Animator playerAnimator;

    public SpriteRenderer spriteRenderer;
    public Sprite unlockedDoorSprite;

    public GameObject particle1, particle2;

    private GameObject afterWinPnl;

    private SaveHandler saveHandler;
    private TimeCounter timeCounter;

    private LevelManager levelManager;

    private void Update()
    {
        manager = GameObject.Find("Manager");
        gameState = manager.GetComponent<GameState>();
        winText = GameObject.Find("WinText");
        player = GameObject.FindWithTag("Player");
        movement = player.GetComponent<PlayerMovement>();
        playerAnimator = player.GetComponent<Animator>();

        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();

        particle1 = GameObject.Find("ParticleSystem1");
        particle2 = GameObject.Find("ParticleSystem2");

        afterWinPnl = GameObject.Find("AfterWin");

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
        if (KeyScript.isPicked == true)
        {
            if (gameState.FromPlayMode == true)
                {
                    spriteRenderer.sprite = unlockedDoorSprite;

                    UnlockedDoor.isWin = true;

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
                    PlayerPrefs.SetFloat("Time", timeCounter.currentTimeCompare);
                    PlayerPrefs.SetString("TimeFormat", TimeCounter.currentTimeText);

                    saveHandler.CompareScore();

                    yield return new WaitForSeconds(2);

                    levelManager.UpdateScoreHandler();

                    yield return new WaitForSeconds(3);

                    afterWinPnl.SetActive(true);
                }
                else
                {
                    spriteRenderer.sprite = unlockedDoorSprite;

                    UnlockedDoor.isWin = true;

                    winText.GetComponent<TMP_Text>().enabled = true;

                    particle1.GetComponent<ParticleSystem>().Play();
                    particle2.GetComponent<ParticleSystem>().Play();

                    playerAnimator.SetFloat("Speed", 0);

                    movement.enabled = false;
                    movement.rb.velocity = new Vector2(0, 0);
                    gameState.GetComponentEnemy(false);
                    gameState.isPlay = false;

                    timeCounter.timerGoing = false;

                    yield return new WaitForSeconds(5f);

                    gameState.StopPlaying();
                }
        }
    }
}