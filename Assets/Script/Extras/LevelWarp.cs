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

	public BoxCollider2D boxCollider;

	public static int currentCoin;

	[SerializeField] private string currentFilename;

	public enum FadeDirection
	{
		In, //Alpha = 1
		Out // Alpha = 0
	}

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
			Debug.Log("Collided");
            if(saveHandler.filename == "TilemapDataLevel1.json")
            {
				NextLevelText.text = "Level 2";
				StartCoroutine(MultiLevelLoad(saveHandler.filename, "TilemapDataLevel2.json"));			
			} else if(saveHandler.filename == "TilemapDataLevel2.json")
            {
				NextLevelText.text = "Level 3";
				StartCoroutine(MultiLevelLoad(saveHandler.filename, "TilemapDataLevel3.json"));
			}
        }
    }


    public IEnumerator MultiLevelLoad(string currentFilename, string nextFilename)
    {
        if(gameState.isPlay == true)
        {
            if(saveHandler.filename == currentFilename)
            {
				fadeOutUIImage.enabled = true;
				NextLevelText.enabled = true;

				yield return new WaitForSeconds(1f);

				tilesCreator.ClearTiles();

				saveHandler.filename = nextFilename;

				PlayerChar.transform.position = playerMovement.startingSpawnPos;

				saveHandler.OnLoad();

				fadeOutUIImage.enabled = false;
				NextLevelText.enabled = false;
			}
        }
		yield return null;
    }
}
