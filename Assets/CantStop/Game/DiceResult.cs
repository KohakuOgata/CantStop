using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

namespace CantStop.Game
{
    public class DiceResult : MonoBehaviourPun
    {
        [SerializeField]
        private TextMeshPro[] numTexts;
        private int[] rootNums = new int[2];
        [SerializeField]
        private Switch bigButton;
        [SerializeField]
        private Switch[] smallButtons = new Switch[2];
        private bool[] canClimb;
        RootManager rootManager;

        private void Start()
        {
            rootManager = RootManager.Instance;

            bigButton.AddListnerOnClicked((x) =>
            {
                photonView.RPC(nameof(OnClickedOthersBigButton), RpcTarget.Others);
                RootManager.Instance.pressedRootButton = bigButton;
                bigButton.onPressed.AddListener(() =>
                {
                    OnPressedBigButton();
                    bigButton.onPressed.RemoveAllListeners();
                });
            });
            smallButtons[0].AddListnerOnClicked((x) =>
            {
                photonView.RPC(nameof(OnClickedOthersSmallButton), RpcTarget.Others, 0);
                RootManager.Instance.pressedRootButton = smallButtons[0];
                smallButtons[0].onPressed.AddListener(() =>
                {
                    OnPressedSmallButton(0);
                    smallButtons[0].onPressed.RemoveAllListeners();
                });
                smallButtons[0].Deactivate();
            });
            smallButtons[1].AddListnerOnClicked((x) =>
            {
                photonView.RPC(nameof(OnClickedOthersSmallButton), RpcTarget.Others, 1);
                RootManager.Instance.pressedRootButton = smallButtons[1];
                smallButtons[1].onPressed.AddListener(() =>
                {
                    OnPressedSmallButton(1);
                    smallButtons[1].onPressed.RemoveAllListeners();
                });
                smallButtons[1].Deactivate();
            });
        }

        [PunRPC]
        private void OnClickedOthersBigButton()
        {
            bigButton.InteractAnimation();
            RootManager.Instance.pressedRootButton = bigButton;
        }

        [PunRPC]
        private void OnClickedOthersSmallButton(int index)
        {
            smallButtons[index].InteractAnimation();
            RootManager.Instance.pressedRootButton = smallButtons[index];
        }

        public void SetRoots(int[] rootNums)
        {
            this.rootNums = rootNums;
            canClimb = new bool[] { GameManager.Instance.CheckCanClimbRoot(rootNums[0]), GameManager.Instance.CheckCanClimbRoot(rootNums[1]) };
            numTexts[0].text = rootNums[0].ToString();
            numTexts[0].color = canClimb[0] ? Color.white : Color.gray;
            numTexts[1].text = rootNums[1].ToString();
            numTexts[1].color = canClimb[1] ? Color.white : Color.gray;
        }

        public void ActivateButtons()
        {
            if (!(canClimb[0] || canClimb[1]))
            {
                bigButton.ring.On();
                return;
            }
            if ((canClimb[0] ^ canClimb[1]))
            {
                if (canClimb[0])
                {
                    smallButtons[0].Activate();
                    return;
                }
                smallButtons[1].Activate();
                return;
            }
            if (GameManager.Instance.climbersNumOnRoot == 2 && !rootManager.GetRoot(rootNums[0]).climbingClimber && !rootManager.GetRoot(rootNums[1]).climbingClimber && rootNums[0] != rootNums[1])
            {
                smallButtons[0].Activate();
                smallButtons[1].Activate();
                return;
            }
            bigButton.Activate();
        }

        public void OnEnterBigButton()
        {
            foreach (var point in RootManager.Instance.GetNextPoints(rootNums))
            {
                point.OnRing();
            }
        }

        public void OnExitBigButton()
        {
            foreach (var point in RootManager.Instance.GetNextPoints(rootNums))
            {
                point.OffRing();
            }
        }

        public void OnEnterSmallButton(int index)
        {
            RootManager.Instance.GetNextPoints(new int[] { rootNums[index] })[0].OnRing();
        }

        public void OnExitSmallButton(int index)
        {
            RootManager.Instance.GetNextPoints(new int[] { rootNums[index] })[0].OffRing();
        }

        public void OnPressedBigButton()
        {
            GameManager.Instance.photonView.RPC(nameof(GameManager.Instance.OnPressedRootButton), RpcTarget.All, rootNums);
        }

        public void OnPressedSmallButton(int index)
        {
            GameManager.Instance.photonView.RPC(nameof(GameManager.Instance.OnPressedRootButton), RpcTarget.All, new int[] { rootNums[index] });
        }

        public void DeactiveAllButtons()
        {
            if (bigButton.active)
                bigButton.Deactivate();
            foreach (var smallButton in smallButtons)
            {
                if (smallButton.active)
                    smallButton.Deactivate();
            }
        }
    }
}
