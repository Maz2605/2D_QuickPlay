using System.Collections;
using _Script.DesignPattern.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : Singleton<GameLoader>
{
    private readonly string _mainMenuSceneName = "MainMenu";    
    private string _currentSceneName;

    protected override void Awake()
    {
        KeepAlive(true);    
        base.Awake();
        if (!SceneManager.GetSceneByName("UICoreScene").isLoaded)
            SceneManager.LoadScene("UICoreScene", LoadSceneMode.Additive);
    }

    public void LoadGame(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;

        _currentSceneName = SceneManager.GetActiveScene().name;

        if (sceneName == _currentSceneName)
        {
            ShowAllGameObjectsInScene(sceneName);
            return;
        }

        HideAllGameObjectsInScene(_currentSceneName);
        StartCoroutine(LoadGameScene(sceneName));
    }

    public void BackToMainMenu()
    {
        StartCoroutine(BackToMainMenuRoutine());
    }

    private IEnumerator BackToMainMenuRoutine()
    {
        yield return AnimTranslate.Instance.PlayTransition();
        
        HideAllGameObjectsInScene(_currentSceneName);
        ShowAllGameObjectsInScene(_mainMenuSceneName);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(_mainMenuSceneName));
        if (_currentSceneName != _mainMenuSceneName)
            yield return UnloadGameScene(_currentSceneName);
        _currentSceneName = _mainMenuSceneName;
        yield return AnimTranslate.Instance.HideTransition();
    }

    private IEnumerator LoadGameScene(string sceneName)
    {
        yield return AnimTranslate.Instance.PlayTransition();

        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            ShowAllGameObjectsInScene(sceneName);
            yield return AnimTranslate.Instance.HideTransition();
            yield break;
        }

        string oldScene = _currentSceneName;
        _currentSceneName = sceneName;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
            yield return null;

        yield return new WaitForSeconds(0.5f);

        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
            yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        ShowAllGameObjectsInScene(sceneName);

        // Chỉ unload scene cũ nếu KHÔNG PHẢI MainMenu
        if (oldScene != _mainMenuSceneName)
            yield return UnloadGameScene(oldScene);

        yield return AnimTranslate.Instance.HideTransition();
    }

    private IEnumerator UnloadGameScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) yield break;

        var scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.IsValid() || !scene.isLoaded) yield break;

        HideAllGameObjectsInScene(sceneName);
        yield return SceneManager.UnloadSceneAsync(sceneName);
    }

    private void HideAllGameObjectsInScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;

        var scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.isLoaded) return;

        foreach (var root in scene.GetRootGameObjects())
            root.SetActive(false);
    }

    private void ShowAllGameObjectsInScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;

        var scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.isLoaded) return;

        foreach (var root in scene.GetRootGameObjects())
            root.SetActive(true);
    }
}
