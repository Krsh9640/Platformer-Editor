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
    public string levelName, scoreDataURL, creatorName;

    public GameObject levelUploadUI;

    public GameObject displayitem, displayLocalSave;

    public static string filePath;

    public GameObject LevelEntryDisplayItem, localLevelEntryDisplayItem;
    public Transform levelDataEntryContent, localLevelDataEntryContent;

    public GameObject UItabs;
    public GameObject sidebar;

    private DownloadScene downloadScene;
    private SaveHandler saveHandler;

    [System.NonSerialized] public string bestPlayer, bestTimeFormat;
    [System.NonSerialized] public ulong creatorIDUlong;
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

    public void PlayerName(int creatorID)
    {
        creatorIDUlong = (ulong)creatorID;
        LootLockerSDKManager.LookupPlayerNamesByPlayerIds(new ulong[] { creatorIDUlong }, response =>
        {
            if (response.success)
            {
                foreach (var player in response.players)
                {
                    creatorName = player.name;
                    Debug.Log(player.name);
                }
            }
            else
            {

            }
        });
    }

    public void TakeScreenshot()
    {
        UItabs.SetActive(false);
        sidebar.SetActive(false);
        string SSfilepath = filePath + levelName + "/Level-Screenshot.png";
        ScreenCapture.CaptureScreenshot(SSfilepath);
        saveHandler.OnSave();
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
    }

    public void UpdateScoreHandler()
    {
        LootLockerSDKManager.RemovingFilesFromAssetCandidates(assetID, scoreFileID, (response) =>
        {
            if (response.success)
            {
                string scoreDataPath = filePath + "/" + levelName + "/ScoreData.json";
                LootLocker.LootLockerEnums.FilePurpose textFileType = LootLocker.LootLockerEnums.FilePurpose.file;

                LootLockerSDKManager.AddingFilesToAssetCandidates(assetID, scoreDataPath, "ScoreData.json", textFileType, (response) =>
                {
                    if (response.success)
                    {
                        Debug.Log("Score deleted and updated");
                        LootLockerSDKManager.UpdatingAnAssetCandidate(assetID, true, (updatedResponse) =>
                        {
                        });
                    }
                    else
                    {

                    }
                });
            }
            else
            {

            }
        });
    }

    public IEnumerator UploadDataAfterMoveFiles(int levelID)
    {
        saveHandler.levelName = levelName;
        saveHandler.MoveFiles();

        yield return new WaitForSeconds(1f);

        UploadLevelData(levelID);
    }

    public void UploadLevelData(int levelID)
    {
        assetID = levelID;
        string screenshotFilePath = filePath + "/" + levelName + "/Level-Screenshot.png";
        LootLocker.LootLockerEnums.FilePurpose screenshotFileType = LootLocker.LootLockerEnums.FilePurpose.primary_thumbnail;

        LootLockerSDKManager.AddingFilesToAssetCandidates(levelID, screenshotFilePath, "Level-Screenshot.png", screenshotFileType, (screnshotResponse) =>
        {
            if (screnshotResponse.success)
            {
                Debug.Log("screenshot uploaded");
                string textFilePath1 = filePath + "/" + levelName + "/TilemapDataLevel1.json";
                LootLocker.LootLockerEnums.FilePurpose textFileType = LootLocker.LootLockerEnums.FilePurpose.file;

                LootLockerSDKManager.AddingFilesToAssetCandidates(levelID, textFilePath1, "TilemapDataLevel1.json", textFileType, (textResponse) =>
                {
                    if (textResponse.success)
                    {
                        Debug.Log("level1 uploaded");
                        string textFilePath2 = filePath + "/" + levelName + "/TilemapDataLevel2.json";

                        LootLockerSDKManager.AddingFilesToAssetCandidates(levelID, textFilePath2, "TilemapDataLevel2.json", textFileType, (textResponse) =>
                        {
                            if (textResponse.success)
                            {
                                Debug.Log("level2 uploaded");
                                string textFilePath3 = filePath + "/" + levelName + "/TilemapDataLevel3.json";

                                LootLockerSDKManager.AddingFilesToAssetCandidates(levelID, textFilePath3, "TilemapDataLevel3.json", textFileType, (textResponse) =>
                                {
                                    if (textResponse.success)
                                    {
                                        Debug.Log("level3 uploaded");
                                        string scoreDataPath = filePath + "/" + levelName + "/ScoreData.json";

                                        LootLockerSDKManager.AddingFilesToAssetCandidates(levelID, scoreDataPath, "ScoreData.json", textFileType, (textResponse) =>
                                        {
                                            if (textResponse.success)
                                            {
                                                LootLockerAssetFile[] assetFiles = textResponse.asset_candidate.files;
                                                for (int i = 0; i < assetFiles.Length; i++)
                                                {
                                                    fileID = assetFiles[i].id;
                                                    scoreFileID = assetFiles[4].id;
                                                }

                                                Debug.Log("score uploaded");
                                                LootLockerSDKManager.UpdatingAnAssetCandidate(levelID, true, (updatedResponse) =>
                                                {
                                                });
                                            }
                                            else
                                            {

                                            }
                                        }
                                        );
                                    }
                                    else
                                    {
                                    }
                                }
                                );
                            }
                            else
                            {
                            }
                        }
                        );
                    }
                    else
                    {
                    }
                }
                );
            }
            else
            {
            }
        }
        );
    }

    public void MyLevelLoader()
    {
        string filepath = Application.persistentDataPath;

        DirectoryInfo dirInfo = new DirectoryInfo(filePath);

        DirectoryInfo[] dirArray = dirInfo.GetDirectories();

        foreach (Transform child in localLevelDataEntryContent)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (DirectoryInfo dir in dirArray)
        {
            string levelName = dir.Name;

            displayLocalSave = Instantiate(localLevelEntryDisplayItem, transform.position, Quaternion.identity);
            displayLocalSave.name = levelName;
            displayLocalSave.transform.SetParent(localLevelDataEntryContent);

            displayLocalSave.GetComponent<LocalLevelEntryData>().levelName = levelName;

            FileInfo[] image = dir.GetFiles("*.png");
            foreach (FileInfo file in image)
            {
                StartCoroutine(LoadLevelIcon(file.FullName, displayLocalSave.GetComponent<LocalLevelEntryData>().levelIcon, levelName));
            }
            displayLocalSave.GetComponent<LocalLevelEntryData>().creatorName = creatorName;
        }
    }

    public void DownloadLevelData()
    {
        LootLockerSDKManager.GetAssetListWithCount(10, (response) =>
        {
            if (response.success)
            {
                foreach (Transform child in levelDataEntryContent)
                {
                    GameObject.Destroy(child.gameObject);
                }

                for (int i = 0; i < response.assets.Length; i++)
                {
                    LootLockerFile[] levelImageFiles = response.assets[i].files;

                    scoreDataURL = levelImageFiles[4].url.ToString();

                    StartCoroutine(DownloadScoreData(response.assets[i].name));

                    GameObject.Find("DownloadSceneManager").GetComponent<DownloadScene>().textFileURL1 = levelImageFiles[1].url.ToString();
                    GameObject.Find("DownloadSceneManager").GetComponent<DownloadScene>().textFileURL2 = levelImageFiles[2].url.ToString();
                    GameObject.Find("DownloadSceneManager").GetComponent<DownloadScene>().textFileURL3 = levelImageFiles[3].url.ToString();

                    StartCoroutine(downloadScene.DownloadLevelTextFile(response.assets[i].name));

                    LootLockerAssetCandidate candidate = response.assets[i].asset_candidate;
                    PlayerName(candidate.created_by_player_id);

                    displayitem = Instantiate(LevelEntryDisplayItem, transform.position, Quaternion.identity);
                    displayitem.name = displayitem.GetComponent<LevelEntryData>().levelName = response.assets[i].name;
                    displayitem.transform.SetParent(levelDataEntryContent);

                    saveHandler.levelName = response.assets[i].name;
                    
                    displayitem.GetComponent<LevelEntryData>().iD = i;
                    displayitem.GetComponent<LevelEntryData>().levelName = response.assets[i].name;

                    displayitem.GetComponent<LevelEntryData>().creatorName = creatorName;

                    displayitem.GetComponent<LevelEntryData>().bestPlayerName = saveHandler.bestPlayerName;
                    displayitem.GetComponent<LevelEntryData>().bestCoin = saveHandler.bestCoin;
                    displayitem.GetComponent<LevelEntryData>().bestTimeFormat = saveHandler.bestTimeFormat;

                    StartCoroutine(LoadLevelIcon(levelImageFiles[0].url.ToString(), displayitem.GetComponent<LevelEntryData>().levelIcon, response.assets[i].name));
                }
            }
        }, null, true, null);
    }

    private IEnumerator DownloadScoreData(string filename)
    {
        UnityWebRequest www = UnityWebRequest.Get(scoreDataURL);
        yield return www.SendWebRequest();

        string filepath = Application.persistentDataPath + "/" + filename;
        if (!Directory.Exists(filepath))
        {
            Directory.CreateDirectory(filepath);
        }
        string newFilepath = filePath + "/" + filename + "/ScoreData.json";
        File.WriteAllText(newFilepath, www.downloadHandler.text);

        yield return new WaitForSeconds(2f);

        saveHandler.LoadScore();
    }

    private IEnumerator LoadLevelIcon(string imageURL, Image levelImage, string filename)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageURL);
        yield return www.SendWebRequest();

        Texture2D loadedImage = DownloadHandlerTexture.GetContent(www);
        levelImage.sprite = Sprite.Create(loadedImage, new Rect(0.0f, 0.0f, loadedImage.width, loadedImage.height), Vector2.zero);
        File.WriteAllBytes(Application.persistentDataPath + "/" + filename + "/Level-Screenshot.png", loadedImage.EncodeToPNG());
    }
}