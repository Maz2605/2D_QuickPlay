using System.Collections;
using _Script.DesignPattern.Singleton;
using DG.Tweening;
using UnityEngine;

public class AnimTranslate : Singleton<AnimTranslate>
{
    public GameObject transitionObject;
    public float zoomDuration = 1f;
    public float startScale = 0.1f;
    public float zoomScale = 5f;

    protected override void Awake()
    {
        KeepAlive(true);
        base.Awake();
    }

    public IEnumerator PlayTransition()
    {
        if (transitionObject == null)
        {
            Debug.LogWarning("transitionObject is null!");
            yield break;
        }

        transitionObject.transform.localScale = Vector3.one * startScale;
        transitionObject.SetActive(true);

        yield return transitionObject.transform
            .DOScale(zoomScale, zoomDuration)
            .SetEase(Ease.InQuad)
            .WaitForCompletion();
    }

    public IEnumerator HideTransition()
    {
        if (transitionObject == null) yield break;

        yield return transitionObject.transform
            .DOScale(startScale, zoomDuration)
            .SetEase(Ease.OutQuad)
            .WaitForCompletion();

        transitionObject.SetActive(false);
    }
}