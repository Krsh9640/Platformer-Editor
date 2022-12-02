using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.IO;

public class DownloadScene : MonoBehaviour
{
    public string textFileURL;

    public LoadingScreen loadScreen;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadLevel()
    {
        StartCoroutine(DownloadLevelTextFile(textFileURL));
    }

    public void PlayLevel()
    {
        StartCoroutine(DownloadLevelTextFile(textFileURL));
        StartCoroutine(PlayLevelOrder());
    }

    private IEnumerator PlayLevelOrder()
    {
        yield return new WaitForSeconds(2.0f);
        GameObject.Find("Manager").GetComponent<GameState>().PlayMode();
    }

    private IEnumerator DownloadLevelTextFile(string textFileURL)
    {
        UnityWebRequest www = UnityWebRequest.Get(textFileURL);
        yield return www.SendWebRequest();

        string filepath = Application.persistentDataPath + "/tilemapData.json";
        File.WriteAllText(filepath, www.downloadHandler.text);

        loadScreen.LoadScene("Loading Screen" ,"Level Editor");
        yield return new WaitForSeconds(1.0f);
        GameObject.Find("Manager").GetComponent<SaveHandler>().OnLoad();
    }
}
