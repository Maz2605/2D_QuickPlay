using _Game.Core.Scripts.SceneFlow;
using _Game.Core.Scripts.UI.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace _Game.Core.Scripts.GameSystem
{
    public abstract class BaseGameController : MonoBehaviour
    {
        [SerializeField] private string mainSceneName = "MainMenu";
        protected void RequestReplay()
        {
            UIManager.Instance.ShowConfirmation(
                "Replay", 
                "Are you sure you want to replay?",
                () =>
                {
                    SceneLoader.Instance.LoadScene(SceneManager.GetActiveScene().name);
                },
                null,
                "Replay",
                "Cancel"
                );
        }

        protected void RequestPause()
        {
            UIManager.Instance.OpenSettings();
        }

        protected void RequestBackHome()
        {
            UIManager.Instance.ShowConfirmation(
                "Back to home",
                "Are you sure",
                () =>
                {
                    SceneLoader.Instance.LoadScene(mainSceneName);
                },
                null);
        }
    }
}