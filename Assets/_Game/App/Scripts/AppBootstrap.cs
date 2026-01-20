using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using _Game.Core.Scripts.SceneFlow; 

namespace _Game.App.Scripts
{
    public class AppBootstrap : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Slider loadingBar;
        [SerializeField] private TextMeshProUGUI percentageText;
        
        [Header("Settings")]
        [SerializeField] private float minBootTime = 2f; 

        private void Start()
        {
            loadingBar.value = 0f;
            percentageText.text = "0%";

            StartCoroutine(BootstrapRoutine());
        }

        private IEnumerator BootstrapRoutine()
        {
            var tween = loadingBar.DOValue(0.8f, minBootTime).SetEase(Ease.OutQuad)
                .OnUpdate(() => percentageText.text = $"{loadingBar.value * 100:F0}%");
            // yield return InitServices(); 
            // yield return InitUserData();
            
            yield return tween.WaitForCompletion();

            SceneLoader.Instance.LoadScene("MainMenu", () =>
            {
                Debug.Log("MainMenu Loaded");
            });
            
            loadingBar.DOValue(1f, 0.5f).OnUpdate(() => percentageText.text = $"{loadingBar.value * 100:F0}%");
            yield return new WaitForSeconds(0.5f);
        }

        private IEnumerator InitServices()
        {
            Debug.Log("Init Ads...");
            yield return new WaitForSeconds(0.5f); 
            Debug.Log("Init Firebase...");
            yield return new WaitForSeconds(0.5f);
        }
    }
}