using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public bool hasLoaded = false;

    public void LoadScene(string FirstScene, string SecondScene)
    {
        StartCoroutine(LoadSceneOrder(FirstScene, SecondScene));
    }

    public IEnumerator LoadSceneOrder(string FirstScene, string SecondScene)
    {
        yield return null;

        AsyncOperation FirstSceneLoad = SceneManager.LoadSceneAsync(FirstScene);

        AsyncOperation SecondSceneLoad = SceneManager.LoadSceneAsync(SecondScene);
        SecondSceneLoad.allowSceneActivation = false;

        while (!SecondSceneLoad.isDone)
        {
            if (SecondSceneLoad.progress >= 0.9f)
            {
                yield return new WaitForSeconds(2.0f);
                SecondSceneLoad.allowSceneActivation = true;
                hasLoaded = true;
            }

            yield return null;
        }
    }
}
