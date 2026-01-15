using _Game.Core.Scripts.SceneFlow;
using UnityEngine;

namespace _Game.App.Scripts
{
    public class SceneLoading : MonoBehaviour
    {
        public enum SceneName
        {
            MainMenu,
            FruitMergeGame,
            BlockBlastGame,
            SudokuGame,
        }

        public void LoadScene(SceneName sceneName = SceneName.MainMenu)
        {
            SceneLoader.Instance.LoadScene(sceneName.ToString());
        }
    }
}
