using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CantStop.Game
{
    public class Root : MonoBehaviour
    {
        [SerializeField]
        private Point[] points;
        private int pointsNum;
        public bool hasBeenClimbed { get; private set; } = false;

        public bool isClimbedNow = false;

        private void Awake()
        {
            pointsNum = points.Length;
        }

        public Point GetNextPoint(PlayerColor color)
        {
            return null;
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
