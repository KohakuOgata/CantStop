using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Props = ExitGames.Client.Photon.Hashtable;
using DG.Tweening;
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

        public const string KeyOrderedPlayers = "o";

        public const int ClimbersNum = 3;

        #endregion

        #region Public Fields

        public static GameState gameState = GameState.BeforeStart;

        public Player[] orderedPlayers;

        public static GameManager Instance;

        public int PlayerCount { get; private set; }

        public int climbersNumOnRoot { get; private set; }

        public int nowPlayerIndex { get; private set; } = 0;

        public Climber[] climbers = new Climber[3];

        public Bell bell;

        public PlayerColor nowColor;

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
        private DiceResult[] diceResults = new DiceResult[3];

        [SerializeField]
        private GameObject resultCanvas;

        [SerializeField]
        private TextMeshProUGUI winnerName;

        [SerializeField]
        private GameObject waitText;

        [SerializeField]
        private GameObject backButton;

        #endregion

        #region Private Fields

        private Player[] players;

        private RootManager rootManager;
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
            rootManager = RootManager.Instance;
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Props{ { PlayerManager.KeyScore, 0} });

            if (!PhotonNetwork.IsMasterClient)
                return;

            var roomProps = new Props() { { KeyNowPlayer, 0 }, { KeyGameState, GameState.BeforeStart } };

            orderedPlayers = players;
            for(int i = 0; i < PlayerCount; i++)
            {
                var random = Random.Range(0, PlayerCount - 1);
                var buf = orderedPlayers[i];
                orderedPlayers[i] = orderedPlayers[random];
                orderedPlayers[random] = buf;
            }
            roomProps.Add(KeyOrderedPlayers, orderedPlayers);

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

        public void BackToLobby()
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.LoadLevel("PrepareGame");
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void OnComplitedRoll(int[] diceNums)
        {
            climbersNumOnRoot = 0;
            foreach(var climber in climbers)
            {
                if (climber.rootNum != -1)
                    climbersNumOnRoot += 1;
            }
            diceResults[0].SetRoots(new int[] { diceNums[0] + diceNums[1], diceNums[2] + diceNums[3] });
            diceResults[1].SetRoots(new int[] { diceNums[0] + diceNums[2], diceNums[1] + diceNums[3] });
            diceResults[2].SetRoots(new int[] { diceNums[0] + diceNums[3], diceNums[1] + diceNums[2] });
            if (orderedPlayers[nowPlayerIndex] != PhotonNetwork.LocalPlayer)
                return;
            bool isAnyButtonOn = false;
            foreach(var diceResult in diceResults)
            {
                isAnyButtonOn |= diceResult.ActivateButtons();
            }
            if (isAnyButtonOn)
                return;
            photonView.RPC(nameof(GoNextTurn), RpcTarget.All);
        }

        public void OnBellClicked()
        {
            photonView.RPC(nameof(OnBellClicked), RpcTarget.All);
        }

        [PunRPC]
        public void OnBellClicked(PhotonMessageInfo info)
        {
            int addScore = 0;
            foreach(var climber in climbers)
            {
                if (climber.rootNum == -1)
                    continue;
                if (rootManager.PutTent(climber.rootNum))
                    addScore += 1;
            }
            if (addScore != 0)
            {
                var nowPlayer = info.Sender;
                var score = (int)nowPlayer.CustomProperties[PlayerManager.KeyScore] + addScore;
                if (score >= 3)
                {
                    resultCanvas.SetActive(true);
                    winnerName.text = nowPlayer.NickName;
                    winnerName.color = PlayerManager.colors[nowColor];
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                        backButton.SetActive(true);
                    else
                        waitText.SetActive(true);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(new Props { { PlayerManager.KeyScore, 0 } });
                    return;
                }
                if(PhotonNetwork.LocalPlayer == nowPlayer)
                    PhotonNetwork.LocalPlayer.SetCustomProperties(new Props { { PlayerManager.KeyScore, score } });
            }
            GoNextTurn();
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
            var root = RootManager.Instance.GetRoot(rootNum);
            return !root.hasBeenClimbed && (climbersNumOnRoot < ClimbersNum || root.climbingClimber != null);
        }

        public void OnCompleteClimb()
        {
            diceSwitch.Activate();
            bell.Activate();
        }

        [PunRPC]
        public void OnPressedRootButton(int[] rootNums, PhotonMessageInfo info)
        {
            Debug.Log("OnPressedRootButton" + rootNums);
            if(info.Sender == PhotonNetwork.LocalPlayer)
            foreach (var diceResult in diceResults)
                diceResult.DeactiveAllButtons();
            rootManager.Climb(rootNums);
        }
        #endregion

        #region Private Method

        [PunRPC]
        private void GoNextTurn()
        {
            if (PhotonNetwork.LocalPlayer == orderedPlayers[nowPlayerIndex])
            {
                bell.Deactivate();
                diceSwitch.Deactivate();
            }
            foreach (var climber in climbers)
            {
                if (climber.rootNum == -1)
                    continue;
                rootManager.GetRoot(climber.rootNum).Reset();
                climber.Reset();
            }
            climbersNumOnRoot = 0;
            nowPlayerIndex = (nowPlayerIndex + 1) % PlayerCount;
            nowPlayerIcon.DOMove(playerNames[nowPlayerIndex].transform.position, .5f);
            nowColor = (PlayerColor)orderedPlayers[nowPlayerIndex].CustomProperties[PlayerManager.KeyColor];
            bell.audio.Play();
            if (PhotonNetwork.LocalPlayer == orderedPlayers[nowPlayerIndex])
            {
                diceSwitch.Activate();
            }

        }

        [PunRPC]
        private void OnCompletedInit()
        {
            orderedPlayers = (Player[])PhotonNetwork.CurrentRoom.CustomProperties[KeyOrderedPlayers];
            for(int i = 0; i < PlayerCount; i++)
            {
                playerNames[i].gameObject.SetActive(true);
                playerNames[i].SetNameText(orderedPlayers[i]);
            }
            nowColor = (PlayerColor)orderedPlayers[nowPlayerIndex].CustomProperties[PlayerManager.KeyColor];
            fade.FadeIn();
            if (orderedPlayers[0] != PhotonNetwork.LocalPlayer)
                return;
            DiceManager.Instance.diceButton.Activate();
        }

        #endregion
    }
}
