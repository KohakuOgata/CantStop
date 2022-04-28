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
    void Start()
    {
        cg = GetComponent<CanvasGroup>();
    }

    public void FadeIn(float fadeTime = DefaultFadeTime)
    {
        cg.DOFade(0, fadeTime).
            OnComplete(() => cg.blocksRaycasts = false);
    }

    public DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> FadeOut(float fadeTime = DefaultFadeTime)
    {
        cg.blocksRaycasts = true;
        return cg.DOFade(1, fadeTime);
    }
}
