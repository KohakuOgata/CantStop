using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace CantStop
{
    public class EmissivalPart : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer meshRenderer;
        [SerializeField]
        private int materialIndex = 0;
        //[SerializeField]
        private Material emissiveMaterial;
        [SerializeField]
        private float fadeTime = 0.3f;
        public Color color = PlayerManager.colors[PlayerColor.None];
        // Start is called before the first frame update
        void Start()
        {
            emissiveMaterial = meshRenderer.materials[materialIndex];
        }

        public void On()
        {
            emissiveMaterial.DOColor(color, fadeTime).SetEase(Ease.Linear);
        }
        public void Off()
        {
            emissiveMaterial.DOColor(Color.black, fadeTime).SetEase(Ease.Linear);
        }

        public void BlinkOff()
        {
            emissiveMaterial.color = color;
            emissiveMaterial.DOColor(Color.black, fadeTime / 2).SetLoops(2, LoopType.Restart).SetEase(Ease.Linear);
        }
    }
}
