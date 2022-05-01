using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;

namespace CantStop.Game
{
    public class Root : MonoBehaviour
    {
        [SerializeField]
        private Point[] points;
        private int pointsNum;
        public bool hasBeenClimbed { get; private set; } = false;
        [HideInInspector]
        public Climber climbingClimber = null;
        [SerializeField]
        private float jumpPower = 0.1f;
        [SerializeField]
        private int rootNum;

        private Dictionary<PlayerColor, int> tentPos = new Dictionary<PlayerColor, int> {
            { PlayerColor.Red, -1} ,
            { PlayerColor.Blue, -1} ,
            { PlayerColor.Green, -1} ,
            { PlayerColor.Yellow, -1} ,
        };

        private void Awake()
        {
            pointsNum = points.Length;
        }

        public void Climb(Climber climber, int forwardNum)
        {
            Debug.Log("Root.Climb" + forwardNum);
            var point = new Point[2] { GetNextPoint(1), GetNextPoint(2) };
            climber.rootNum = rootNum;
            climber.pointNum += forwardNum;
            climbingClimber = climber;
            var color = (PlayerColor)GameManager.Instance.orderedPlayers[GameManager.Instance.nowPlayerIndex].CustomProperties[PlayerManager.KeyColor];
            var sequence = climber.transform.DOJump(point[0].sockets[color].position, jumpPower, 1, 0.5f).SetEase(Ease.Linear);
            point[0].OnRing();
            if (forwardNum == 1)
            {
                sequence.OnComplete(RootManager.Instance.OnCompleteClimbeAnimation);
                return;
            }
            sequence.
                Append(climber.transform.DOJump(point[1].sockets[color].position, jumpPower, 1, 0.5f).SetEase(Ease.Linear)).
                OnComplete(RootManager.Instance.OnCompleteClimbeAnimation);
            point[1].OnRing();
        }

        public Point GetNextPoint(int forwardNum)
        {
            if (climbingClimber)
            {
                return points[Mathf.Min(climbingClimber.pointNum + forwardNum, pointsNum - 1)];
            }
            return points[Mathf.Min(tentPos[(PlayerColor)GameManager.Instance.orderedPlayers[GameManager.Instance.nowPlayerIndex].CustomProperties[PlayerManager.KeyColor]] + forwardNum, pointsNum - 1)];
        }

        public void OnEmissive()
        {
            foreach(var point in points)
            {
                point.OnRing();
            }
        }

        public void OffEmissive()
        {
            foreach(var point in points)
            {
                point.OffRing();
            }
        }
    }
}
