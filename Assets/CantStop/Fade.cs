using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Fade : MonoBehaviour
{
    [HideInInspector]
    public CanvasGroup cg;
    const float DefaultFadeTime = 5;
    // Start is called before the first frame update
    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
    }

    public void FadeIn(float fadeTime = DefaultFadeTime)
    {
        cg.blocksRaycasts = false;
        cg.DOFade(0, fadeTime);
    }

    public void FadeOut(TweenCallback onComplete = null)
    {
        cg.blocksRaycasts = true;
        var tween = cg.DOFade(1, DefaultFadeTime);
        if (onComplete == null)
            return;
        tween.OnComplete(onComplete);
    }
}
