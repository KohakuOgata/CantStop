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
        
        private bool isRollingDice = false;
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            

        }

        public void Roll()
        {
            isRollingDice = true;
            
        }
    }
}
