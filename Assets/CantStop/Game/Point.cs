using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace CantStop.Game
{
    public class Point : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer meshRenderer;
        private Material RingMaterial;
        [SerializeField, ColorUsage(true, true)]
        private Color emissionColor;
        [SerializeField]
        private float fadeTime = 0.3f;
        [SerializeField]
        private Transform redSocket;
        [SerializeField]
        private Transform blueSocket;
        [SerializeField]
        private Transform greenSocket;
        [SerializeField]
        private Transform yellowSocket;

        public Dictionary<PlayerColor, Transform> sockets;

        private void Awake()
        {
            RingMaterial = meshRenderer.materials[1];
            RingMaterial.EnableKeyword("_EMISSION");
            sockets = new Dictionary<PlayerColor, Transform>
            {
                {PlayerColor.Red, redSocket },
                {PlayerColor.Blue, blueSocket },
                {PlayerColor.Green, greenSocket },
                {PlayerColor.Yellow, yellowSocket },
            };
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
    }
}
