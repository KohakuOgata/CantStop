using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;

namespace CantStop
{
    public class Bell : MonoBehaviourPun
    {
        [HideInInspector]
        public new AudioSource audio { get; private set; }
        [SerializeField]
        private Transform bellTop;
        private new Collider collider;
        private EmissivalPart emissival;

        private void Start()
        {
            audio = GetComponent<AudioSource>();
            collider = GetComponent<SphereCollider>();
            collider.enabled = false;
            emissival = GetComponent<EmissivalPart>();
        }

        public void Activate()
        {
            collider.enabled = true;
            emissival.On();
        }

        public void Deactivate()
        {
            collider.enabled = false;
            emissival.Off();
        }

        public void ClickedAnimation()
        {
            audio.Play();
            bellTop.DOLocalMoveY(0, 0.25f).SetLoops(2, LoopType.Yoyo);
        }
    }
}
