using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace CantStop.Game
{
    public class RootManager : MonoBehaviour
    {
        [SerializeField]
        private Root[] roots = new Root[10];
        public static RootManager Instance { get; private set; }
        private int climbingNum;
        private int completeClimbeNum = 0;
        public Switch pressedRootButton;

        private void Awake()
        {
            Instance = this;
        }

        public Root GetRoot(int rootNum)
        {
            return roots[rootNum - 2];
        }

        public Point[] GetNextPoints(int[] rootNums)
        {
            if(rootNums.Length == 1)
                return new Point[] { GetRoot(rootNums[0]).GetNextPoint(1) };
            if (rootNums[0] == rootNums[1])
                return new Point[] { GetRoot(rootNums[0]).GetNextPoint(2) };
            return new Point[] { GetRoot(rootNums[0]).GetNextPoint(1), GetRoot(rootNums[1]).GetNextPoint(1) };
        }

        public void Climb(int[] rootNums)
        {
            Debug.Log("RootManager.Climb" + rootNums);
            climbingNum = 1;
            if(rootNums.Length == 2)
            {
                if(rootNums[0] == rootNums[1])
                {
                    Climb(rootNums[0], 2);
                    return;
                }
                climbingNum = 2;
            }
            foreach(var rootNum in rootNums)
            {
                Climb(rootNum, 1);
            }
        }

        private void Climb(int rootNum, int forwardNum)
        {
            Debug.Log("RootManager.Climb" + rootNum + "," + forwardNum);
            var root = GetRoot(rootNum);
            Climber climber = null;
            if (root.climbingClimber)
                climber = root.climbingClimber;
            else
                foreach (var c in GameManager.Instance.climbers)
                    if (c.rootNum == -1)
                    {
                        climber = c;
                        break;
                    }
            root.Climb(climber, forwardNum);
        }

        public void OnCompleteClimbeAnimation()
        {
            completeClimbeNum += 1;
            if (completeClimbeNum < climbingNum)
                return;
            completeClimbeNum = 0;
            //pressedRootButton.OffCenter();
            pressedRootButton.InteractAnimation();

            if (GameManager.Instance.orderedPlayers[GameManager.Instance.nowPlayerIndex] != PhotonNetwork.LocalPlayer)
                return;
            GameManager.Instance.OnCompleteClimb();
        }
    }
}
