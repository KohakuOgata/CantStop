using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Props = ExitGames.Client.Photon.Hashtable;

namespace CantStop.Game
{
    public enum GameState
    {
        BeforeStart,
        Rolling
    }
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Public Const Fields

        public const string KeyNowPlayer = "n";

        public const string KeyGameState = "s";

        public const string KeyPlayerOrder = "o";

        #endregion

        #region Public Fields

        public static GameState gameState = GameState.BeforeStart;

        public static Player[] playerOrder;

        public static GameManager Instance;

        public static int PlayerCount { get; private set; }

        #endregion

        #region Private Serialize Fields

        [SerializeField]
        private List<GamePlayerName> playerNames = new List<GamePlayerName>();

        [SerializeField]
        private Switch diceSwitch;

        [SerializeField]
        private Transform nowPlayerIcon;

        #endregion

        #region Private Fields


        #endregion

        #region MonoBehaviour Callbacks

        private void Start()
        {

            Instance = this;

            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene("Launcher");

                return;
            }

            PlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;

            if (!PhotonNetwork.IsMasterClient)
                return;

            var roomProps = new Props() { { KeyNowPlayer, 0 }, { KeyGameState, GameState.BeforeStart } };

            playerOrder = new Player[PlayerCount];
            var players = PhotonNetwork.PlayerList;
            for(int i = 0; i < PlayerCount; i++)
            {
                playerOrder[i] = players[i];
            }
            for(int i = 0; i < PlayerCount; i++)
            {
                var random = Random.Range(0, PlayerCount - 1);
                var buf = playerOrder[i];
                playerOrder[i] = playerOrder[random];
                playerOrder[random] = buf;
            }
            roomProps.Add(KeyPlayerOrder, playerOrder);

            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);

            photonView.RPC(nameof(OnCompletedInit), RpcTarget.AllViaServer);
        }

        #endregion

        #region Pun Callbacks
        public override void OnLeftRoom()
        {
            Debug.Log("OnLeftRoom()");
            Destroy(PlayerManager.LocalPlayerInstance);
            SceneManager.LoadScene("Launcher");
        }

        #endregion

        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void OnComplitedRoll(int[] diceNums)
        {

        }

        public void OnPressedDiceButton()
        {
            
        }

        #endregion

        #region Private Method

        [PunRPC]
        private void OnCompletedInit()
        {
            playerOrder = (Player[])PhotonNetwork.CurrentRoom.CustomProperties[KeyPlayerOrder];
            for(int i = 0; i < PlayerCount; i++)
            {
                playerNames[i].gameObject.SetActive(true);
                playerNames[i].SetNameText(playerOrder[i]);
            }
            if (playerOrder[0] != PhotonNetwork.LocalPlayer)
                return;
            DiceManager.Instance.diceButton.Activate();
        }

        #endregion
    }
}
