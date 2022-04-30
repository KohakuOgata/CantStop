using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Props = ExitGames.Client.Photon.Hashtable;
using TMPro;

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

        public const int ClimbersNum = 3;

        #endregion

        #region Public Fields

        public static GameState gameState = GameState.BeforeStart;

        public int[] playerOrder;

        public static GameManager Instance;

        public int PlayerCount { get; private set; }

        public int climbersNumOnRoot { get; private set; }

        public Root[] roots = new Root[10];

        #endregion

        #region Private Serialize Fields

        [SerializeField]
        private List<GamePlayerName> playerNames = new List<GamePlayerName>();

        [SerializeField]
        private Switch diceSwitch;

        [SerializeField]
        private Transform nowPlayerIcon;

        [SerializeField]
        private Fade fade;

        [SerializeField]
        private Climber[] climbers = new Climber[3];

        [SerializeField]
        private Transform[] climberSockets = new Transform[3];

        [SerializeField]
        private DiceResult[] diceResults = new DiceResult[3];

        #endregion

        #region Private Fields

        private Player[] players;
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
            fade.gameObject.SetActive(true);
            players = PhotonNetwork.PlayerList;

            if (!PhotonNetwork.IsMasterClient)
                return;

            var roomProps = new Props() { { KeyNowPlayer, 0 }, { KeyGameState, GameState.BeforeStart } };

            playerOrder = new int[PlayerCount];
            for(int i = 0; i < PlayerCount; i++)
            {
                playerOrder[i] = i;
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
            diceResults[0].SetRoots(new int[] { diceNums[0] + diceNums[1], diceNums[2] + diceNums[3] });
            diceResults[1].SetRoots(new int[] { diceNums[0] + diceNums[2], diceNums[1] + diceNums[3] });
            diceResults[2].SetRoots(new int[] { diceNums[0] + diceNums[3], diceNums[1] + diceNums[2] });
        }

        public void OnPressedDiceButton()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootNum">2~12</param>
        /// <returns></returns>
        public bool CheckCanClimbRoot(int rootNum)
        {
            var root = roots[rootNum - 2];
            return !root.hasBeenClimbed && (climbersNumOnRoot < ClimbersNum || root.isClimbedNow);
        }

        public void OnPressedRootButton(int[] rootNums)
        {

        }

        public void OnPressedRootButton(int rootNum)
        {

        }
        #endregion

        #region Private Method

        [PunRPC]
        private void OnCompletedInit()
        {
            playerOrder = (int[])PhotonNetwork.CurrentRoom.CustomProperties[KeyPlayerOrder];
            for(int i = 0; i < PlayerCount; i++)
            {
                playerNames[i].gameObject.SetActive(true);
                playerNames[i].SetNameText(players[playerOrder[i]]);
            }
            fade.FadeIn();
            if (players[playerOrder[0]] != PhotonNetwork.LocalPlayer)
                return;
            DiceManager.Instance.diceButton.Activate();
        }

        #endregion
    }
}
