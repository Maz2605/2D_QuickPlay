using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TextMeshProUGUI percentageText;
    [SerializeField] private float loadingDuration = 2f;


    private void Start()
    {
        CheckCamera();
        StartCoroutine(loadingCoroutine());
    }
    
    private void CheckCamera()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            if(canvas.worldCamera == null || canvas.worldCamera != Camera.main)
            {
                canvas.worldCamera = Camera.main;
            }
        }
    }

    private IEnumerator loadingCoroutine()
    {
        loadingBar.value = 0f;
        percentageText.text = "0%";
        
        loadingBar.DOValue(1f, loadingDuration).SetEase(Ease.InOutQuad);
        float displayedPercentage = 0f;
        DOTween.To(() => displayedPercentage, x => displayedPercentage = x, 100f, loadingDuration)
            .SetEase(Ease.InOutQuad)
            .OnUpdate(() =>
            {
                percentageText.text = $"{displayedPercentage:F0}%";
            });
        yield return new WaitForSeconds(loadingDuration);
        yield return new WaitForSeconds(0.5f);
        GameLoader.Instance.LoadMainMenuScene();
    }
}
