using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DownloadScene : MonoBehaviour
{
    public string textFileURL1, textFileURL2, textFileURL3;

    public LoadingScreen loadScreen;

    private static GameObject instance;

    private SaveHandler saveHandler;

    private Authenticator authenticator;

    public GameObject homeButtons;

    public GameState gameState;

    [System.NonSerialized] public bool doneLoad, hasLoggedin;

    public void CheckInit()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = gameObject;
        DontDestroyOnLoad(this);
    }

    private void Awake()
    {
        CheckInit();
        saveHandler = GetComponent<SaveHandler>();
        authenticator = GameObject.Find("Authenticator").GetComponent<Authenticator>();

        Scene scene = SceneManager.GetActiveScene();

        if (hasLoggedin == false)
        {
            authenticator.CheckSession();
            hasLoggedin = true;
        }
    }

    private void Update()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (scene.name == "Level Editor")
        {
            doneLoad = true;
        }
        else if (scene.name == "Main Menu")
        {
            doneLoad = false;
        }
    }

    public void LoadLevel()
    {
        StartCoroutine(LoadLevelRoutine());
    }

    public void LocalLoadLevel()
    {
        string filepath = Application.persistentDataPath + "/Downloaded" + "/" + saveHandler.levelName;
        StartCoroutine(LocalLoadLevelRoutine(filepath));
    }

    public IEnumerator LocalLoadLevelRoutine(string filename)
    {
        loadScreen.LoadScene("Loading Screen", "Level Editor");
        saveHandler.levelName = filename;
        saveHandler.filename = "TilemapDataLevel1.json";

        yield return new WaitForSeconds(0.6f);

        saveHandler.OnLoad();
    }

    public IEnumerator LoadLevelRoutine()
    {
        StartCoroutine(DownloadLevelTextFile(saveHandler.levelName));

        loadScreen.LoadScene("Loading Screen", "Level Editor");
        saveHandler.filename = "TilemapDataLevel1.json";

        yield return new WaitForSeconds(0.6f);

        saveHandler.OnLoad();
    }

    public void PlayLevel()
    {
        StartCoroutine(PlayLevelRoutine());
    }

    public IEnumerator PlayLevelRoutine()
    {
        StartCoroutine(DownloadLevelTextFile(saveHandler.levelName));

        loadScreen.LoadScene("Loading Screen", "Level Editor");

        yield return new WaitForSeconds(0.6f);

        saveHandler.filename = "TilemapDataLevel1.json";
        saveHandler.OnLoad();

        StartCoroutine(PlayLevelOrder());
    }

    private IEnumerator PlayLevelOrder()
    {
        gameState = GameObject.Find("Manager").GetComponent<GameState>();

        if (gameState != null && saveHandler.filename != null)
        {
            if (doneLoad == true)
            {
                yield return new WaitForSeconds(0.5f);
                gameState.PlayMode();
            }
        }
    }

    public IEnumerator DownloadLevelTextFile(string filename)
    {
        string filepath = Path.Combine(Application.persistentDataPath, filename);
        if (!Directory.Exists(filepath))
        {
            Directory.CreateDirectory(filepath);
        }

        UnityWebRequest www1 = UnityWebRequest.Get(textFileURL1);
        yield return www1.SendWebRequest();

        string filepath1 = filepath + "/TilemapDataLevel1.json";
        File.WriteAllText(filepath1, www1.downloadHandler.text);

        UnityWebRequest www2 = UnityWebRequest.Get(textFileURL2);
        yield return www2.SendWebRequest();

        string filepath2 = filepath + "/TilemapDataLevel2.json";
        File.WriteAllText(filepath2, www2.downloadHandler.text);

        UnityWebRequest www3 = UnityWebRequest.Get(textFileURL3);
        yield return www3.SendWebRequest();

        string filepath3 = filepath + "/TilemapDataLevel3.json";
        File.WriteAllText(filepath3, www3.downloadHandler.text);
    }
}