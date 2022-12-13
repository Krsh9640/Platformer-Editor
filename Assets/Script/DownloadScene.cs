using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadScene : MonoBehaviour
{
    public string textFileURL1, textFileURL2, textFileURL3;

    public LoadingScreen loadScreen;

    private static GameObject instance;

    private SaveHandler saveHandler;

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
    }

    public void LoadLevel()
    {
        StartCoroutine(DownloadLevelTextFile());
    }

    public void PlayLevel()
    {
        StartCoroutine(DownloadLevelTextFile());
        StartCoroutine(PlayLevelOrder());
    }

    private IEnumerator PlayLevelOrder()
    {
        yield return new WaitForSeconds(2.0f);
        GameObject.Find("Manager").GetComponent<GameState>().PlayMode();
    }

    private IEnumerator DownloadLevelTextFile()
    {
        UnityWebRequest www1 = UnityWebRequest.Get(textFileURL1);
        yield return www1.SendWebRequest();

        string filepath1 = Application.persistentDataPath + "/TilemapDataLevel1.json";
        File.WriteAllText(filepath1, www1.downloadHandler.text);

        UnityWebRequest www2 = UnityWebRequest.Get(textFileURL2);
        yield return www2.SendWebRequest();

        string filepath2 = Application.persistentDataPath + "/TilemapDataLevel2.json";
        File.WriteAllText(filepath2, www2.downloadHandler.text);

        UnityWebRequest www3 = UnityWebRequest.Get(textFileURL3);
        yield return www3.SendWebRequest();

        string filepath3 = Application.persistentDataPath + "/TilemapDataLevel3.json";
        File.WriteAllText(filepath3, www3.downloadHandler.text);

        loadScreen.LoadScene("Loading Screen", "Level Editor");

        yield return new WaitForSeconds(3.0f);

        saveHandler.filename = "TilemapDataLevel1.json";
        saveHandler.OnLoad();
    }
}