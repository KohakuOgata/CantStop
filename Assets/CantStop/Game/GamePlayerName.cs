using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

namespace CantStop.Game
{
    public class GamePlayerName : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro textMesh;

        public void SetNameText(Player player)
        {
            textMesh.text = player.NickName;
            textMesh.color = PlayerManager.colors[(PlayerColor)player.CustomProperties[PlayerManager.KeyColor]];
        }
    }
}
