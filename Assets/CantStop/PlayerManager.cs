using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using Props = ExitGames.Client.Photon.Hashtable;

namespace CantStop
{
    public enum PlayerColor
    {
        None,
        Red,
        Blue,
        Green,
        Yellow,
    }

    public class PlayerManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields

        public static readonly Dictionary<PlayerColor, Color> colors = new Dictionary<PlayerColor, Color>() {
           {PlayerColor.None, new Color32(255, 255, 255, 255) },
           {PlayerColor.Red, new Color32(255, 107, 107, 255) },
           {PlayerColor.Blue, new Color32(77, 150, 255, 255) },
           {PlayerColor.Green, new Color32(107, 203, 119, 255) }, 
           {PlayerColor.Yellow, new Color32(255, 217, 61, 255) }, };

        public static GameObject LocalPlayerInstance;

        public const string KeyColor = "c";

        public const string KeyScore = "s";

        #endregion

        #region MonoBehaviour CallBacks

        public void Awake()
        {
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Props() { { KeyColor, PlayerColor.None } });
                Debug.Log("Local Players Color Inited");
            }

            DontDestroyOnLoad(gameObject);
        }

        public override void OnDisable()
        {
            // Always call the base to remove callbacks
            base.OnDisable();

        }

        #endregion

    }
}