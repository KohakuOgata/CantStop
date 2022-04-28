using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;
using TMPro;

namespace CantStop.Game
{
    public class DiceManager : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private TextMeshPro[] dices;

        [SerializeField]
        public Switch diceButton;
        
        private bool isRollingDice = false;

        public static DiceManager Instance { get; private set; }
        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
        }

        // Update is called once per frame
        void Update()
        {
            RollingUpdate();
        }

        public void Roll()
        {
            photonView.RPC(nameof(StartRolling), RpcTarget.All);
            DOTween.Sequence().
                AppendInterval(1).
                AppendCallback(() =>
                {
                    var nums = new int[4];
                    for(int i =0; i < 4; i++)
                    {
                        nums[i] = Random.Range(1, 6);
                    }
                    photonView.RPC(nameof(EndRolling), RpcTarget.All, nums);
                    GameManager.Instance.OnComplitedRoll(nums);
                });
        }

        void RollingUpdate()
        {
            if (!isRollingDice)
                return;
            foreach(var dice in dices)
            {
                dice.text = Random.Range(1, 6).ToString();
            }
        }

        [PunRPC]
        void StartRolling()
        {
            isRollingDice = true;
            diceButton.Deactivate();
            diceButton.OnCenter();
        }

        [PunRPC]
        void EndRolling(int[] nums)
        {
            isRollingDice = false;
            diceButton.OffCenter();
            for (int i = 0; i < 4; i++)
            {
                dices[i].text = nums[i].ToString();
            }
        }
    }
}
