using System.Collections.Generic;
using _Game.Core.Scripts.Audio.Manager;
using _Game.Core.Scripts.Data;
using _Game.Core.Scripts.SceneFlow;
using _Game.Core.Scripts.UI.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Game.App.Scripts.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private List<GameProfileSO> allGames; 
        [SerializeField] private MiniGameItemView itemPrefab;
        [SerializeField] private Transform contentContainer;
        [SerializeField] private BaseAudioController audioController;
        private void Start()
        {
            foreach(var profile in allGames)
            {
                var item = Instantiate(itemPrefab, contentContainer);
                item.Setup(profile, OnGameSelected);
            }
            
            if(audioController != null) audioController.Initialize();
        }

        private void OnGameSelected(GameProfileSO profile)
        {
            if (profile.id.ToLower().Contains("none"))
            {
                UIManager.Instance.ShowToast("You are going to the comming soon!");
                return;
            }
            SceneLoader.Instance.LoadScene(profile.sceneName);
        }
    }
}