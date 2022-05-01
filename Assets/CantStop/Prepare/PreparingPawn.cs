using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;
using Props = ExitGames.Client.Photon.Hashtable;

namespace CantStop.Prepare
{
    public class PreparingPawn : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        public PlayerColor color;
        [SerializeField]
        private Transform centerStandSocket;
        [SerializeField]
        private Transform myStandSocket;
        public Player owner;
        [SerializeField]
        private float moveTime = 0.5f;
        [SerializeField]
        private Ease ease;
        [SerializeField]
        private float hoverHeight = 0.1f;
        [SerializeField]
        new Collider collider;
        [SerializeField]
        Transform modelSocket;

        private bool isHovering = false;

        // Start is called before the first frame update
        private void Awake()
        {
        }
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void OnPointerEnter()
        {
            if (owner != null)
                return;
            modelSocket.DOLocalMove(Vector3.up * hoverHeight, moveTime).SetEase(Ease.OutQuad);
            isHovering = true;
        }

        public void OnPointExit()
        {
            if (!isHovering)
                return;
            modelSocket.DOMove(transform.parent.position, moveTime).SetEase(Ease.InQuad);
        }

        public void OnClicked()
        {
            if (owner != null)
            {
                if (owner != PhotonNetwork.LocalPlayer)
                {
                    Debug.Log("That pawn is already owned");
                    return;
                }
                photonView.RPC(nameof(OnReturned), RpcTarget.AllViaServer, true);
                return;
            }
            var localPlayerColor = (PlayerColor)PhotonNetwork.LocalPlayer.CustomProperties[PlayerManager.KeyColor];
            if (localPlayerColor != PlayerColor.None)
            {
                PrepareManager.Instance.pawns[localPlayerColor].photonView.RPC(nameof(OnReturned), RpcTarget.AllViaServer, false);
            }
            photonView.RPC(nameof(OnGot), RpcTarget.AllViaServer);
        }

        [PunRPC]
        public void OnReturned(bool changePlayerColor)
        {
            if (owner == null)
                return;
            transform.parent = centerStandSocket;
            collider.enabled = false;
            var sequence = DOTween.Sequence();
            sequence.
                Append(transform.DOMove(centerStandSocket.position, moveTime).SetEase(ease)).
                AppendCallback(() => collider.enabled = true);
            if (changePlayerColor)
            {
                var stand = PrepareManager.Instance.GetStand(owner.ActorNumber);
                sequence.AppendCallback(() => stand.Release());
                if (PhotonNetwork.IsMasterClient)
                {
                    owner.SetCustomProperties(new Props() { { PlayerManager.KeyColor, (PlayerColor)PlayerColor.None } });
                }
            }
            owner = null;
        }

        [PunRPC]
        void OnGot(PhotonMessageInfo info)
        {
            if (info.Sender == owner)
                return;
            owner = info.Sender;
            collider.enabled = false;

            if (info.Sender == PhotonNetwork.LocalPlayer)
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Props() { { PlayerManager.KeyColor, color } });
            }
            transform.parent = GetSocket(info.Sender);
            transform.DOMove(transform.parent.position, moveTime).SetEase(ease).
                OnComplete(() =>
                {
                    collider.enabled = true;
                    PrepareManager.Instance.GetStand(info.Sender.ActorNumber).Press(color);
                });
            Debug.Log(info.Sender.NickName + " gets " + color + " pawn");
        }

        public void OnGot(Player gotBy)
        {
            if (gotBy == owner)
                return;
            owner = gotBy;
            transform.parent = GetSocket(gotBy);
            transform.position = transform.parent.position;
        }

        Transform GetSocket(Player owner)
        {
            return PrepareManager.Instance.GetStand(owner.ActorNumber).socket;
        }
    }
}

