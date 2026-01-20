using System.Collections.Generic;
using _Game.Core.Scripts.Data;
using _Game.Core.Scripts.SceneFlow;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Game.App.Scripts.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private List<GameProfileSO> allGames; 
        [SerializeField] private MiniGameItemView itemPrefab;
        [SerializeField] private Transform contentContainer;
        
        private void Start()
        {
            foreach(var profile in allGames)
            {
                var item = Instantiate(itemPrefab, contentContainer);
                item.Setup(profile, OnGameSelected);
            }
        }

        private void OnGameSelected(GameProfileSO profile)
        {
            SceneLoader.Instance.LoadScene(profile.sceneName);
        }
    }
}