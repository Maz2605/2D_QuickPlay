using System.Collections;
using System.Collections.Generic;
using _Script.DesignPattern.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : Singleton<GameLoader>
{
    private readonly string _mainMenuSceneName = "MainMenu";
    private readonly string _loadingScenceName = "Loading";
    
    private string _currentSceneName;   
    protected override void Awake()
    {
        KeepAlive(true);
        base.Awake();
    }

    public void LoadMainMenuScene()
    {
        if (string.IsNullOrEmpty(_mainMenuSceneName)) return;
        HideAllGameObjectsInScene(SceneManager.GetActiveScene().name);
        StartCoroutine(LoadGameScene(_mainMenuSceneName));
        ShowAllGameObjectsInScene(_mainMenuSceneName);
    }
    public void BackToMainMenu()
    {
        HideAllGameObjectsInScene("GamePlay");
        ShowAllGameObjectsInScene(_mainMenuSceneName);
        
    }
    
    private IEnumerator LoadGameScene(string sceneName = "MainMenu")
    {
        if (string.IsNullOrEmpty(sceneName)) yield break;

        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            ShowAllGameObjectsInScene(sceneName);
            yield break;
        }
        
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        
        ShowAllGameObjectsInScene(sceneName);
        yield return UnloadGameScene(_loadingScenceName);
    }
      
    
    private IEnumerator UnloadGameScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) yield break;
        var scene = SceneManager.GetSceneByName(sceneName);
        if(!scene.isLoaded || !scene.IsValid()) yield break;
        HideAllGameObjectsInScene(sceneName);
        yield return SceneManager.UnloadSceneAsync(sceneName);
    }
    
    private void HideAllGameObjectsInScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;

        var scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.isLoaded) return;
        foreach (var rootGameObject in scene.GetRootGameObjects())
        {
            rootGameObject.SetActive(false);
        }
    }
    
    private void ShowAllGameObjectsInScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;

        var scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.isLoaded) return;
        foreach (var rootGameObject in scene.GetRootGameObjects())
        {
            rootGameObject.SetActive(true);
        }
    }
}
