using System.Collections;
using _Game.Core.Scripts.UI;
using _Script.DesignPattern.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Game.Core.Scripts.SceneFlow
{
    public class SceneLoader : Singleton<SceneLoader>
    {
        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneRoutine(sceneName));
        }

        private IEnumerator LoadSceneRoutine(string sceneName)
        {
            UIManager.Instance.ToggleLoadingScreen(true);

            yield return new WaitForSecondsRealtime(0.5f);

            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            op.allowSceneActivation = false;

            while (op.progress < 0.9f)
            {
                yield return null;
            }

            op.allowSceneActivation = true;
            while (!op.isDone)
            {
                yield return null;
            }

            yield return new WaitForSecondsRealtime(0.5f);
            UIManager.Instance.ToggleLoadingScreen(false);
            
            Time.timeScale = 1f; 
        }
    }
}
