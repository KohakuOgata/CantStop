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

        public IEnumerator Roll()
        {
            photonView.RPC(nameof(StartRolling), RpcTarget.All);
            yield return new WaitForSeconds(1);
            var nums = new int[4];
            for (int i = 0; i < 4; i++)
            {
                var random = Random.Range(0, 1200) % 6 + 1;
                if(i == 0)
                {
                    nums[i] = random;
                    continue;
                }
                nums[i] = (nums[i - 1] + random) % 6 + 1;
            }
            photonView.RPC(nameof(EndRolling), RpcTarget.All, nums);
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
        public void StartRolling()
        {
            isRollingDice = true;
        }

        [PunRPC]
        void EndRolling(int[] nums)
        {
            isRollingDice = false;
            diceButton.InteractAnimation();
            for (int i = 0; i < 4; i++)
            {
                dices[i].text = nums[i].ToString();
            }
            GameManager.Instance.OnComplitedRoll(nums);
        }
    }
}
