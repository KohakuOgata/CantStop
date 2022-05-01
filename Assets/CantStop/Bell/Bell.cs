using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;

namespace CantStop
{
    public class Bell : MonoBehaviourPun
    {

        private new AudioSource audio;
        [SerializeField]
        private Transform bellTop;
        private new Collider collider;
        private MeshRenderer meshRenderer;
        private Material ringMaterial;

        private void Start()
        {
            audio = GetComponent<AudioSource>();
            collider = GetComponent<SphereCollider>();
            collider.enabled = false;
            meshRenderer = GetComponent<MeshRenderer>();
            ringMaterial = meshRenderer.materials[3];
            ringMaterial.EnableKeyword("_EMISSION");
        }

        public void Activate()
        {
            collider.enabled = true;
            var nowR = ringMaterial.color.r;
            DOTween.To(
                () => nowR,
                (x) => ringMaterial.SetColor("_EmissionColor", new Color(x, x, x, 1)),
                1,
                .5f * (1.0f - nowR / 1));
        }

        public void Deactivate()
        {
            collider.enabled = false;
            var nowR = ringMaterial.color.r;
            DOTween.To(
                () => nowR,
                (x) => ringMaterial.SetColor("_EmissionColor", new Color(x, x, x, 1)),
                0,
                .5f * (nowR / 1));
        }

        public void ClickedAnimation()
        {
            audio.Play();
            bellTop.DOLocalMoveY(0, 0.25f).SetLoops(2, LoopType.Yoyo);
        }
    }
}
