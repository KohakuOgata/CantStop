using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace CantStop.Game
{
    public class Climber : MonoBehaviour
    {
        public int rootNum = -1;
        public int pointNum = -1;
        [SerializeField]
        private Transform homeSocket;

        public void Reset()
        {
            if (rootNum != -1)
            {
                transform.DOJump(homeSocket.position, .1f, 1, .5f).SetEase(Ease.Linear);
            }
            rootNum = -1;
            pointNum = -1;
        }
    }
}
