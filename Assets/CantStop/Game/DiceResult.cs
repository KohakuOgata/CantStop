using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CantStop.Game
{
    public class DiceResult : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro[] numTexts;
        private int[] rootNums = new int[2];
        [SerializeField]
        private Switch bigButton;
        [SerializeField]
        private Switch[] smallButtons = new Switch[2];

        private void Awake()
        {
            bigButton.AddListnerOnClicked((x) =>
            {
                OnPressedBigButton();
                
                bigButton.Deactivate();
            });
        }

        public void SetRoots(int[] rootNums)
        {
            this.rootNums = rootNums;
            var canClimbe = new bool[] { GameManager.Instance.CheckCanClimbRoot(rootNums[0]), GameManager.Instance.CheckCanClimbRoot(rootNums[1]) };
            numTexts[0].text = rootNums[0].ToString();
            numTexts[0].color = canClimbe[0] ? Color.white : Color.gray;
            numTexts[1].text = rootNums[1].ToString();
            numTexts[1].color = canClimbe[1] ? Color.white : Color.gray;
            if(!(canClimbe[0] || canClimbe[1]))
            {
                bigButton.OffRing();
                return;
            }
            if ((canClimbe[0] ^ canClimbe[1]))
            {
                if (canClimbe[0])
                {
                    smallButtons[0].Activate();
                    return;
                }
                smallButtons[1].Activate();
                return;
            }
            if(GameManager.Instance.climbersNumOnRoot == 2 && !GameManager.Instance.roots[rootNums[0] - 2].isClimbedNow && !GameManager.Instance.roots[rootNums[0] - 2].isClimbedNow)
            {
                smallButtons[0].Activate();
                smallButtons[1].Activate();
                return;
            }
            bigButton.Activate();
        }

        public void OnEnterBigButton()
        {
            GameManager.Instance.roots[rootNums[0] - 2].OnEmissive();
            GameManager.Instance.roots[rootNums[1] - 2].OnEmissive();
        }

        public void OnExitBigButton()
        {
            GameManager.Instance.roots[rootNums[0] - 2].OffEmissive();
            GameManager.Instance.roots[rootNums[1] - 2].OffEmissive();
        }

        public void OnEnterSmallButton(int index)
        {
            GameManager.Instance.roots[rootNums[index] - 2].OnEmissive();
        }

        public void OnExitSmallButton(int index)
        {
            GameManager.Instance.roots[rootNums[index] - 2].OffEmissive();
        }

        public void OnPressedBigButton()
        {
            GameManager.Instance.OnPressedRootButton(rootNums);
        }

        public void OnPressedSmallButton(int index)
        {
            GameManager.Instance.OnPressedRootButton(rootNums[index]);
        }
    }
}
