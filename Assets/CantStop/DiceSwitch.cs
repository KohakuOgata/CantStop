using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace CantStop.Game
{
    public class DiceSwitch : MonoBehaviourPun
    {
        private Switch sw;
        // Start is called before the first frame update
        void Start()
        {
            sw = GetComponent<Switch>();
            sw.AddListnerOnClicked((x) =>
            {
                Debug.Log("DiceSwitch OnClickedCallback Called");
                photonView.RPC(nameof(OnPressedOthers), RpcTarget.Others);
                sw.onPressed.AddListener(() => {
                    StartCoroutine(DiceManager.Instance.Roll());
                    sw.onPressed.RemoveAllListeners();
                });
                sw.Deactivate();
            });
        }

        [PunRPC]
        void OnPressedOthers()
        {
            sw.InteractAnimation();
        }
    }
}
