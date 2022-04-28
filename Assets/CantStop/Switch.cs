using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace CantStop
{
    public class Switch : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer meshRenderer;
        private Material RingMaterial;
        private Material CenterMaterial;
        [SerializeField, ColorUsage(true, true)]
        private Color emissionColor;
        [SerializeField]
        private float fadeTime = 0.3f;
        private Collider myCollider;

        private void Start()
        {
            RingMaterial = meshRenderer.materials[1];
            CenterMaterial = meshRenderer.materials[2];
            RingMaterial.EnableKeyword("_EMISSION");
            CenterMaterial.EnableKeyword("_EMISSION");
            myCollider = GetComponent<BoxCollider>();
            myCollider.enabled = false;
        }

        public void OnRing()
        {
            DOTween.To( 
                () => RingMaterial.color.r,
                (x) => RingMaterial.SetColor("_EmissionColor", new Color(x, x, x, 1)),
                emissionColor.r,
                fadeTime * (1.0f - RingMaterial.color.r / emissionColor.r));
        }

        public void OffRing()
        {
            DOTween.To(
                () => RingMaterial.color.r,
                (x) => RingMaterial.SetColor("_EmissionColor", new Color(x, x, x, 1)),
                0,
                fadeTime).
                SetEase(Ease.InQuad);
        }

        public void OnCenter()
        {
            DOTween.To(
                () => CenterMaterial.color.r,
                (x) => CenterMaterial.SetColor("_EmissionColor", new Color(x, x, x, 1)),
                emissionColor.r,
                fadeTime * (1.0f - CenterMaterial.color.r / emissionColor.r));
        }

        public void OffCenter()
        {
            DOTween.To(
                () => CenterMaterial.color.r,
                (x) => CenterMaterial.SetColor("_EmissionColor", new Color(x, x, x, 1)),
                0,
                fadeTime).
                SetEase(Ease.InQuad);
        }

        public void Activate()
        {
            OnRing();
            myCollider.enabled = true;
        }

        public void Deactivate()
        {
            OffRing();
            myCollider.enabled = false;
        }
    }
}
