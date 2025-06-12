using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    /// <summary>
    ///  unloadSceneIndex = -1 means that will unload the current active scene
    /// </summary>
    /// <param name="loadSceneIndex"></param>
    /// <param name="unloadSceneIndex"></param>
    public static void LoadEndSessionSceneAndUnloadGameplay(int loadSceneIndex, int unloadSceneIndex = -1)
    {
        int currentIndex = unloadSceneIndex;
        if (currentIndex == -1)
        {
            currentIndex = SceneManager.GetActiveScene().buildIndex;
        }
        else
        {
            currentIndex = unloadSceneIndex;
        }
        Debug.Log(currentIndex);
        Instance.StartCoroutine(Instance.LoadAndUnloadRoutine(loadSceneIndex, currentIndex));
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator LoadAndUnloadRoutine(int loadScene, int unloadScene)
    {
        AsyncOperation loadSceneVar = SceneManager.LoadSceneAsync(loadScene, LoadSceneMode.Additive);

        while (!loadSceneVar.isDone)
            yield return null;

        AsyncOperation unloadSceneVar = SceneManager.UnloadSceneAsync(unloadScene);

        while (!unloadSceneVar.isDone)
            yield return null;

    }
}
