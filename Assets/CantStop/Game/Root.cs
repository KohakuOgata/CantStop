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
            var point = new Point[2] { GetNextPoint(1), GetNextPoint(2) };
            climbingClimber = climber;
            climbingClimber.pointNum = Mathf.Min(pointsNum - 1, Mathf.Max(tentPos[GameManager.Instance.nowColor], climbingClimber.pointNum) + forwardNum);
            climber.rootNum = rootNum;
            var sequence = climber.transform.DOJump(point[0].sockets[GameManager.Instance.nowColor].position, jumpPower, 1, 0.5f).SetEase(Ease.Linear);
            point[0].OnRing();
            if (forwardNum == 1)
            {
                sequence.OnComplete(RootManager.Instance.OnCompleteClimbeAnimation);
                return;
            }
            sequence.
                Append(climber.transform.DOJump(point[1].sockets[GameManager.Instance.nowColor].position, jumpPower, 1, 0.5f).SetEase(Ease.Linear)).
                OnComplete(RootManager.Instance.OnCompleteClimbeAnimation);
            if (point[0] != point[1])
                point[1].OnRing();
        }

        public Point GetNextPoint(int forwardNum)
        {
            if (climbingClimber)
            {
                return points[Mathf.Min(climbingClimber.pointNum + forwardNum, pointsNum - 1)];
            }
            return points[Mathf.Min(tentPos[GameManager.Instance.nowColor] + forwardNum, pointsNum - 1)];
        }

        public void OnEmissive()
        {
            foreach (var point in points)
            {
                point.OnRing();
            }
        }

        public void OffEmissive()
        {
            foreach (var point in points)
            {
                point.OffRing();
            }
        }

        public bool PutTent()
        {
            if (tentPos[GameManager.Instance.nowColor] != -1)
                points[tentPos[GameManager.Instance.nowColor]].RemoveTent(GameManager.Instance.nowColor);
            points[climbingClimber.pointNum].PutTent();
            tentPos[GameManager.Instance.nowColor] = climbingClimber.pointNum;
            if (climbingClimber.pointNum < pointsNum - 1)
                return false;
            foreach (var pos in tentPos)
            {
                if (pos.Key == GameManager.Instance.nowColor || pos.Value == -1)
                    continue;
                points[pos.Value].RemoveTent(pos.Key);
            }
            var nowColor = PlayerManager.colors[GameManager.Instance.nowColor];
            foreach (var point in points)
            {
                point.emissival.ChangeColor(nowColor);
            }
            return hasBeenClimbed = true;
        }

        public void Reset()
        {
            if (climbingClimber.pointNum < pointsNum - 1)
                foreach (var point in points)
                    point.OffRing();

            climbingClimber = null;

        }
    }
}
