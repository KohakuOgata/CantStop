using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace CantStop.Game
{
    public class Point : MonoBehaviour
    {
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

        public EmissivalPart emissival { get; private set; }

        public Dictionary<PlayerColor, Transform> sockets;

        private void Awake()
        {
            sockets = new Dictionary<PlayerColor, Transform>
            {
                {PlayerColor.Red, redSocket },
                {PlayerColor.Blue, blueSocket },
                {PlayerColor.Green, greenSocket },
                {PlayerColor.Yellow, yellowSocket },
            };
        }
        private void Start()
        {
            emissival = GetComponent<EmissivalPart>();
        }
        public void OnRing()
        {
            emissival.On();
        }

        public void OffRing()
        {
            emissival.Off();
        }

        public void PutTent()
        {
            var socket = sockets[GameManager.Instance.nowColor];
            socket.DOScale(1, fadeTime);
            socket.DORotate(Vector3.zero, fadeTime);
        }

        public void RemoveTent(PlayerColor color)
        {
            var socket = sockets[color];
            socket.DOScale(0, fadeTime);
            socket.DORotate(new Vector3(0, 360, 0), fadeTime);
        }

        public void RemoveAllTents()
        {
            foreach(var socket in sockets)
            {
                socket.Value.DOScale(0, fadeTime);
                socket.Value.DORotate(new Vector3(0, 360, 0), fadeTime);
            }
        }
    }
}
