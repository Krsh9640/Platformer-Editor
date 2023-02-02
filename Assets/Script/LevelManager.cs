
using LootLocker.Requests;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class LevelManager : MonoBehaviour
{
    public TMP_InputField levelNameInput;
    public string levelName, scoreDataURL;

    public GameObject levelUploadUI, notifUploadComplete;

    public GameObject displayitem, displayLocalSave;

    public static string filePath;

    public GameObject LevelEntryDisplayItem, localLevelEntryDisplayItem;
    public Transform levelDataEntryContent, localLevelDataEntryContent;

    public GameObject UItabs;
    public GameObject sidebar;

    private DownloadScene downloadScene;
    private SaveHandler saveHandler;

    [System.NonSerialized] public string bestPlayer, bestTimeFormat;
    [System.NonSerialized] public int fileID, assetID, scoreFileID, bestCoin;

    private void Awake()
    {
        filePath = Application.persistentDataPath;

        downloadScene = GameObject.Find("DownloadSceneManager").GetComponent<DownloadScene>();
        saveHandler = GameObject.Find("DownloadSceneManager").GetComponent<SaveHandler>();

        Scene scene = SceneManager.GetActiveScene();
    }

    public void SaveButton()
    {
        saveHandler.OnSave();
    }

    public void LoadButton()
    {
        saveHandler.OnLoad();
    }

    public void CreateLevel()
    {
        levelName = levelNameInput.text;

        LootLockerSDKManager.CreatingAnAssetCandidate(levelName, (response) =>
        {
            if (response.success)
            {
                StartCoroutine(UploadDataAfterMoveFiles(response.asset_candidate_id));
            }
            else
            {
            }
        });

        levelUploadUI.SetActive(false);
    }

    public void TakeScreenshot()
    {
        UItabs.SetActive(false);
        sidebar.SetActive(false);
        string SSfilepath = filePath + "/" + saveHandler.levelNameOnly + "/Level-Screenshot.png";

        ScreenCapture.CaptureScreenshot(SSfilepath);
    }

    private IEnumerator waitScreenshot()
    {
        TakeScreenshot();
        yield return new WaitForSeconds(0f);
        UItabs.SetActive(true);
        sidebar.SetActive(true);
        levelUploadUI.SetActive(true);
    }

    public void OpenUploadLevelUI()
    {
        StartCoroutine(waitScreenshot());
        saveHandler.OnSave();
    }

    public void UpdateScoreHandler()
    {
        LootLockerSDKManager.RemovingFilesFromAssetCandidates(assetID, scoreFileID, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Score removed");

                string scoreDataPath = filePath + "/" + levelName + "/ScoreData.json";
                LootLocker.LootLockerEnums.FilePurpose textFileType = LootLocker.LootLockerEnums.FilePurpose.file;

                if (scoreDataPath != null)
                {
                    LootLockerSDKManager.AddingFilesToAssetCandidates(assetID, scoreDataPath, "ScoreData.json", textFileType, (response) =>
                    {
                        if (response.success)
                        {
                            Debug.Log("Score added");
                            LootLockerSDKManager.UpdatingAnAssetCandidate(assetID, true, (updatedResponse) =>
                            {
                                Debug.Log("Score updated");
                            });
                        }
                    });
                }
            }
        });
    }

    public IEnumerator UploadDataAfterMoveFiles(int levelID)
    {
        saveHandler.levelName = levelName;
        saveHandler.MoveFilesCheck();

        if (saveHandler.moveLevelComplete == true)
        {
            saveHandler.SaveScoreToDefault();
            saveHandler.moveLevelComplete = false;
        }
        yield return new WaitForSeconds(1f);

        UploadLevelData(levelID);
    }

    public void UploadLevelData(int levelID)
    {
        assetID = levelID;
        string screenshotFilePath = filePath + "/" + levelName + "/Level-Screenshot.png";
        LootLocker.LootLockerEnums.FilePurpose screenshotFileType = LootLocker.LootLockerEnums.FilePurpose.primary_thumbnail;

        if (screenshotFilePath != null)
        {
            LootLockerSDKManager.AddingFilesToAssetCandidates(levelID, screenshotFilePath, "Level-Screenshot.png", screenshotFileType, (screnshotResponse) =>
            {
                if (screnshotResponse.success)
                {
                    Debug.Log("screenshot uploaded");

                    string textFilePath1 = filePath + "/" + levelName + "/TilemapDataLevel1.json";
                    LootLocker.LootLockerEnums.FilePurpose textFileType = LootLocker.LootLockerEnums.FilePurpose.file;

                    if (textFilePath1 != null)
                    {
                        LootLockerSDKManager.AddingFilesToAssetCandidates(levelID, textFilePath1, "TilemapDataLevel1.json", textFileType, (textResponse) =>
                        {
                            if (textResponse.success)
                            {
                                Debug.Log("level1 uploaded");

                                string textFilePath2 = filePath + "/" + levelName + "/TilemapDataLevel2.json";

                                if (textFilePath2 != null)
                                {
                                    LootLockerSDKManager.AddingFilesToAssetCandidates(levelID, textFilePath2, "TilemapDataLevel2.json", textFileType, (textResponse) =>
                                    {
                                        if (textResponse.success)
                                        {
                                            Debug.Log("level2 uploaded");

                                            string textFilePath3 = filePath + "/" + levelName + "/TilemapDataLevel3.json";

                                            if (textFilePath3 != null)
                                            {
                                                LootLockerSDKManager.AddingFilesToAssetCandidates(levelID, textFilePath3, "TilemapDataLevel3.json", textFileType, (textResponse) =>
                                                {
                                                    if (textResponse.success)
                                                    {
                                                        Debug.Log("level3 uploaded");

                                                        string scoreDataPath = filePath + "/" + levelName + "/ScoreData.json";

                                                        if (scoreDataPath != null)
                                                        {
                                                            LootLockerSDKManager.AddingFilesToAssetCandidates(levelID, scoreDataPath, "ScoreData.json", textFileType, (textResponse) =>
                                                            {
                                                                if (textResponse.success)
                                                                {
                                                                    LootLockerAssetFile[] assetFiles = textResponse.asset_candidate.files;
                                                                    for (int i = 0; i < assetFiles.Length; i++)
                                                                    {
                                                                        fileID = assetFiles[i].id;
                                                                        scoreFileID = assetFiles[4].id;

                                                                        LootLockerSDKManager.UpdatingAnAssetCandidate(levelID, true, (updatedResponse) =>
                                                                        {
                                                                            notifUploadComplete.SetActive(true);
                                                                        });
                                                                    }
                                                                    Debug.Log("score uploaded");
                                                                }
                                                            });
                                                        }
                                                    }
                                                });
                                            }
                                        }
                                    });
                                }
                            }
                        });
                    }
                }
            });
        }
    }

    public void MyLevelLoader()
    {
        string localFilepath = Application.persistentDataPath;

        DirectoryInfo dirInfo = new DirectoryInfo(localFilepath);

        DirectoryInfo[] dirArray = dirInfo.GetDirectories();

        foreach (Transform child in localLevelDataEntryContent)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (DirectoryInfo dir in dirArray)
        {
            if (dir.Name != "Downloaded")
            {
                string levelName = dir.Name;

                displayLocalSave = Instantiate(localLevelEntryDisplayItem, transform.position, Quaternion.identity);
                displayLocalSave.name = levelName;
                displayLocalSave.transform.SetParent(localLevelDataEntryContent);

                displayLocalSave.GetComponent<LocalLevelEntryData>().levelName = levelName;

                FileInfo[] image = dir.GetFiles("*.png");
                foreach (FileInfo file in image)
                {
                    StartCoroutine(LocalLoadLevelIcon(file.FullName, displayLocalSave.GetComponent<LocalLevelEntryData>().levelIcon, levelName));
                }
            }
        }
    }

    public void DownloadLevelData()
    {

        StartCoroutine(DownloadLevelDataRoutine());
    }

    public IEnumerator DownloadLevelDataRoutine()
    {
        LootLockerSDKManager.GetAssetListWithCount(20, (response) =>
        {
            if (response.success)
            {
                foreach (Transform child in levelDataEntryContent)
                {
                    if (child != null)
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                    else
                    {
                        continue;
                    }
                }

                for (int i = 0; i < response.assets.Length; i++)
                {
                    LootLockerFile[] levelImageFiles = response.assets[i].files;

                    scoreDataURL = levelImageFiles[4].url.ToString();

                    StartCoroutine(DownloadScoreData(response.assets[i].name));

                    StartCoroutine(downloadScene.DownloadLevelTextFile(response.assets[i].name, levelImageFiles[1].url.ToString(), levelImageFiles[2].url.ToString(), levelImageFiles[3].url.ToString()));

                    displayitem = Instantiate(LevelEntryDisplayItem, transform.position, Quaternion.identity);
                    displayitem.name = displayitem.GetComponent<LevelEntryData>().levelName = response.assets[i].name;
                    displayitem.transform.SetParent(levelDataEntryContent);

                    saveHandler.levelName = response.assets[i].name;
                    saveHandler.levelNameOnly = response.assets[i].name;

                    displayitem.GetComponent<LevelEntryData>().iD = i;
                    displayitem.GetComponent<LevelEntryData>().levelName = response.assets[i].name;

                    displayitem.GetComponent<LevelEntryData>().creatorText.text = saveHandler.LoadScoreCreatorName();

                    displayitem.GetComponent<LevelEntryData>().bestPlayerNameText.text = saveHandler.LoadScoreBestPlayerName();
                    displayitem.GetComponent<LevelEntryData>().bestPlayerCoinText.text = saveHandler.LoadScoreBestCoin().ToString();
                    displayitem.GetComponent<LevelEntryData>().bestTimeFormat = saveHandler.LoadScoreTimeFormat();

                    StartCoroutine(LoadLevelIcon(levelImageFiles[0].url.ToString(), displayitem.GetComponent<LevelEntryData>().levelIcon, response.assets[i].name));
                }
            }
        }, null, true, null);

        yield return null;
    }

    private IEnumerator DownloadScoreData(string filename)
    {
        UnityWebRequest www = UnityWebRequest.Get(scoreDataURL);
        yield return www.SendWebRequest();

        string newFilepath = Application.persistentDataPath + "/Downloaded" + "/" + filename + "/ScoreData.json";
        Debug.Log(newFilepath);
        if (!Directory.Exists(Application.persistentDataPath + "/Downloaded/" + filename))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Downloaded/" + filename);
        }

        File.WriteAllText(newFilepath, www.downloadHandler.text);
    }

    private IEnumerator LoadLevelIcon(string imageURL, Image levelImage, string filename)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageURL);
        yield return www.SendWebRequest();

        Texture2D loadedImage = DownloadHandlerTexture.GetContent(www);
        levelImage.sprite = Sprite.Create(loadedImage, new Rect(0.0f, 0.0f, loadedImage.width, loadedImage.height), Vector2.zero);
        File.WriteAllBytes(Application.persistentDataPath + "/Downloaded" + "/" + filename + "/Level-Screenshot.png", loadedImage.EncodeToPNG());
    }

    private IEnumerator LocalLoadLevelIcon(string imageURL, Image levelImage, string filename)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageURL);
        yield return www.SendWebRequest();

        Texture2D loadedImage = DownloadHandlerTexture.GetContent(www);
        levelImage.sprite = Sprite.Create(loadedImage, new Rect(0.0f, 0.0f, loadedImage.width, loadedImage.height), Vector2.zero);
        File.WriteAllBytes(Application.persistentDataPath + "/" + filename + "/Level-Screenshot.png", loadedImage.EncodeToPNG());
    }
}