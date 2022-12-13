using LootLocker.Requests;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public TMP_InputField levelNameInput;
    private string levelName;

    public GameObject levelUploadUI;

    public GameObject displayitem;

    public static string filePath;

    public GameObject LevelEntryDisplayItem;
    public Transform levelDataEntryContent;

    public GameObject UItabs;
    public GameObject sidebar;

    private GameObject DownloadSceneManager;
    private SaveHandler saveHandler;

    private void Awake()
    {
        filePath = Application.persistentDataPath;

        DownloadSceneManager = GameObject.Find("DownloadSceneManager");
        saveHandler = DownloadSceneManager.GetComponent<SaveHandler>();

        Scene scene = SceneManager.GetActiveScene();
        Debug.Log(scene.name);
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
                UploadLevelData(response.asset_candidate_id);
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
        ScreenCapture.CaptureScreenshot(filePath + "/Level-Screenshot.png");
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

    public void UploadLevelData(int levelID)
    {
        string screenshotFilePath = filePath + "/Level-Screenshot.png";
        LootLocker.LootLockerEnums.FilePurpose screenshotFileType = LootLocker.LootLockerEnums.FilePurpose.primary_thumbnail;

        LootLockerSDKManager.AddingFilesToAssetCandidates(levelID, screenshotFilePath, "Level-Screenshot.png", screenshotFileType, (screnshotResponse) =>
        {
            if (screnshotResponse.success)
            {
                string textFilePath1 = filePath + "/TilemapDataLevel1.json";
                LootLocker.LootLockerEnums.FilePurpose textFileType = LootLocker.LootLockerEnums.FilePurpose.file;

                LootLockerSDKManager.AddingFilesToAssetCandidates(levelID, textFilePath1, "TilemapDataLevel1.json", textFileType, (textResponse) =>
                {
                    if (textResponse.success)
                    {
                        string textFilePath2 = filePath + "/TilemapDataLevel2.json";
                        LootLocker.LootLockerEnums.FilePurpose textFileType = LootLocker.LootLockerEnums.FilePurpose.file;

                        LootLockerSDKManager.AddingFilesToAssetCandidates(levelID, textFilePath2, "TilemapDataLevel2.json", textFileType, (textResponse) =>
                        {
                            if (textResponse.success)
                            {
                                string textFilePath3 = filePath + "/TilemapDataLevel3.json";
                                LootLocker.LootLockerEnums.FilePurpose textFileType = LootLocker.LootLockerEnums.FilePurpose.file;

                                LootLockerSDKManager.AddingFilesToAssetCandidates(levelID, textFilePath3, "TilemapDataLevel3.json", textFileType, (textResponse) =>
                                {
                                    if (textResponse.success)
                                    {
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
                    displayitem = Instantiate(LevelEntryDisplayItem, transform.position, Quaternion.identity);
                    displayitem.name = displayitem.GetComponent<LevelEntryData>().levelName = response.assets[i].name;
                    displayitem.transform.SetParent(levelDataEntryContent);

                    displayitem.GetComponent<LevelEntryData>().iD = i;
                    displayitem.GetComponent<LevelEntryData>().levelName = response.assets[i].name;

                    LootLockerFile[] levelImageFiles = response.assets[i].files;
                    StartCoroutine(LoadLevelIcon(levelImageFiles[0].url.ToString(), displayitem.GetComponent<LevelEntryData>().levelIcon));
                    GameObject.Find("DownloadSceneManager").GetComponent<DownloadScene>().textFileURL1 = levelImageFiles[1].url.ToString();
                    GameObject.Find("DownloadSceneManager").GetComponent<DownloadScene>().textFileURL2 = levelImageFiles[2].url.ToString();
                    GameObject.Find("DownloadSceneManager").GetComponent<DownloadScene>().textFileURL3 = levelImageFiles[3].url.ToString();
                }
            }
        }, null, true);
    }

    private IEnumerator LoadLevelIcon(string imageURL, Image levelImage)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageURL);
        yield return www.SendWebRequest();

        Texture2D loadedImage = DownloadHandlerTexture.GetContent(www);
        levelImage.sprite = Sprite.Create(loadedImage, new Rect(0.0f, 0.0f, loadedImage.width, loadedImage.height), Vector2.zero);
    }
}