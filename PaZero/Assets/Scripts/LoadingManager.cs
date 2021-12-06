using System;

using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    private static string loadingSceneName = "Loading";

    private static string sceneToLoad, sceneToUnload;

    private static Animator loadingScreen;

    public static event Action OnFinishedLoading;

    #if UNITY_EDITOR
        public static bool hasLoadedOnce = false;
    #endif

    /// <summary>
    ///     Starting loading a scene, with a transition and loading screen.
    ///     If no scene to unload is specified, it unloads the current active scene.
    /// </summary>
    public static void LoadScene(string sceneToLoad, string sceneToUnload = null)
    {
        if(sceneToUnload == null)
            sceneToUnload = SceneManager.GetActiveScene().name;

        LoadingManager.sceneToLoad = sceneToLoad;
        LoadingManager.sceneToUnload = sceneToUnload;

        // Start process by loading the loading/transition scene
        AsyncOperation loading = SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);
        loading.completed += LoadingScene;

        #if UNITY_EDITOR
            hasLoadedOnce = true;
        #endif
    }

    public static void ReloadActiveScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    private static void LoadingScene(AsyncOperation loading)
    {
        loading.completed -= LoadingScene;

        // Get loading screen for animations
        loadingScreen = GameObject.FindWithTag(loadingSceneName).GetComponent<Animator>();
    }

    private static void FinishLoading(AsyncOperation nextScene)
    {
        nextScene.completed -= FinishLoading;

        // Transition out of the loading screen
        loadingScreen.SetBool(loadingSceneName, false);

        // Set new scene as active
        Scene scene = SceneManager.GetSceneByName(sceneToLoad);
        SceneManager.SetActiveScene(scene);

        // Reset time scale
        Time.timeScale = 1f;
    }

    // Called by animation event
    private void UnloadLastScene()
    {
        // Start unloading the last scene
        AsyncOperation lastScene = SceneManager.UnloadSceneAsync(sceneToUnload);
        lastScene.completed += LoadNextScene;
    }

    private void LoadNextScene(AsyncOperation lastScene)
    {
        lastScene.completed -= LoadNextScene;

        try
        {
            // Start loading the next scene
            AsyncOperation nextScene = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
            nextScene.completed += FinishLoading;
        }
        catch(Exception ex)
        {
            Debug.LogWarning(ex);
            Debug.LogWarning("Scene not found, going to menu.");

            // Start loading the next scene
            AsyncOperation nextScene = SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);
            nextScene.completed += FinishLoading;
        }
    }

    // Called by animation event
    private void UnloadLoadingScreen()
    {
        OnFinishedLoading?.Invoke();

        // Start unloading the loading scene
        SceneManager.UnloadSceneAsync(loadingSceneName);
    }
}
