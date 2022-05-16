using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace CantStop
{
    public class EmissivalPart : MonoBehaviour
    {
        public Color color = PlayerManager.colors[PlayerColor.None];

        [SerializeField]
        private MeshRenderer meshRenderer;
        [SerializeField]
        private int materialIndex = 0;
        [SerializeField]
        private float fadeTime = 0.3f;

        private Material emissiveMaterial;
        private int emissiveColorKey;
        private Color nowColor = Color.black;

        void Start()
        {
            emissiveMaterial = meshRenderer.materials[materialIndex];
            emissiveMaterial.EnableKeyword("_EMISSION");
            emissiveColorKey = Shader.PropertyToID("_EmissionColor");
        }

        public void On()
        {
            //emissiveMaterial.DOColor(color, fadeTime).SetEase(Ease.Linear);
            DOTween.Sequence().
                Join(DOTween.To(
                () => nowColor.r,
                (x) => nowColor.r = x,
                color.r,
                fadeTime
                )).

                Join(DOTween.To(
                () => nowColor.g,
                (x) => nowColor.g = x,
                color.g,
                fadeTime
                )).

                Join(DOTween.To(
                () => nowColor.b,
                (x) => nowColor.b = x,
                color.b,
                fadeTime
                )).

                OnUpdate(() => emissiveMaterial.SetColor(emissiveColorKey, nowColor * nowColor * 4));
        }
        public void Off()
        {
            //emissiveMaterial.DOColor(Color.black, fadeTime).SetEase(Ease.Linear);
            DOTween.Sequence().
                Join(DOTween.To(
                () => nowColor.r,
                (x) => nowColor.r = x,
                0,
                fadeTime
                )).

                Join(DOTween.To(
                () => nowColor.g,
                (x) => nowColor.g = x,
                0,
                fadeTime
                )).

                Join(DOTween.To(
                () => nowColor.b,
                (x) => nowColor.b = x,
                0,
                fadeTime
                )).

                OnUpdate(() => emissiveMaterial.SetColor(emissiveColorKey, nowColor * nowColor * 4));
        }

        public void ChangeColor(Color newColor)
        {
            color = newColor;
            On();
        }
    }
}
