using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;

namespace CantStop.Prepare
{
    public class Bell : MonoBehaviourPun
    {

        private AudioSource audio;
        [SerializeField]
        private Transform bellTop;

        private void Start()
        {
            audio = GetComponent<AudioSource>();
        }

        public void OnClicked()
        {
            photonView.RPC("RPCOnClicked", RpcTarget.All);
        }

        [PunRPC]
        void RPCOnClicked(PhotonMessageInfo info)
        {
            audio.Play();
            bellTop.DOLocalMoveY(0, 0.25f).SetLoops(2, LoopType.Yoyo);
            PrepareManager.Instance.OnBellClicked();
        }
    }
}
