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

        public const string ColorKey = "c";

        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour CallBacks

        public void Awake()
        {
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Props() { { ColorKey, PlayerColor.None } });
                Debug.Log("Local Players Color Inited");
            }

            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        public void Start()
        {
        }


        public override void OnDisable()
        {
            // Always call the base to remove callbacks
            base.OnDisable();

        }

        public void Update()
        {
            // we only process Inputs and check health if we are the local player
            if (photonView.IsMine)
            {

            }
        }

#if !UNITY_5_4_OR_NEWER
        /// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }
#endif
        #endregion

        #region Public Static Methods

        public static PlayerColor IntToColor(int i)
        {
            return (PlayerColor)System.Enum.ToObject(typeof(PlayerColor), i);
        }

        #endregion

        #region Private Methods


#if UNITY_5_4_OR_NEWER

#endif

        #endregion

        #region IPunObservable implementation


        #endregion
    }
}