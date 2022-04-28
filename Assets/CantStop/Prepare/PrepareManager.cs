using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;
using Props = ExitGames.Client.Photon.Hashtable;

namespace CantStop.Prepare
{
    public class PrepareManager : MonoBehaviourPunCallbacks
    {

        #region Public Fields

        static public PrepareManager Instance;

        public Dictionary<PlayerColor, PreparingPawn> pawns = new Dictionary<PlayerColor, PreparingPawn>();

        #endregion

        #region Private Serialize Fields

        [SerializeField]
        private GameObject playerPrefab;

        [SerializeField]
        private GameObject pawnStandPrefab;

        [SerializeField]
        private List<Transform> othersStandSockets = new List<Transform>();

        [SerializeField]
        private PawnStand myStand;

        [SerializeField]
        private Fade fade;

        [SerializeField]
        private float standPivotDepth;

        [SerializeField]
        private float standPivotHeight;

        [SerializeField]
        private float standAppearTime = 1f;

        #endregion

        #region privarte Fields

        private readonly Dictionary<int, PawnStand> otherStands = new Dictionary<int, PawnStand>();

        private float pivotToStandAngle;

        private float pivotToStandDistance;

        #endregion

        #region MonoBehaviour Callbacks
        void Start()
        {
            Instance = this;

            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene("Launcher");

                return;
            }

            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

                    PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity, 0);
                }
                else
                {

                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }

            pawns.Add(PlayerColor.Red, GameObject.Find("RedPreparePawn").GetComponent<PreparingPawn>());
            pawns.Add(PlayerColor.Blue, GameObject.Find("BluePreparePawn").GetComponent<PreparingPawn>());
            pawns.Add(PlayerColor.Green, GameObject.Find("GreenPreparePawn").GetComponent<PreparingPawn>());
            pawns.Add(PlayerColor.Yellow, GameObject.Find("YellowPreparePawn").GetComponent<PreparingPawn>());

            pivotToStandAngle = Mathf.Atan2(standPivotHeight, -standPivotDepth);
            pivotToStandDistance = Mathf.Sqrt(standPivotHeight * standPivotHeight + standPivotDepth * standPivotDepth);

            myStand.SetNameText(PhotonNetwork.LocalPlayer.NickName);
            for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount - 1; i++)
            {
                var other = PhotonNetwork.PlayerListOthers[i];
                SpawnStand(other, i).Init(other);
                var othersColor = (PlayerColor)other.CustomProperties[PlayerManager.ColorKey];
                if (othersColor == PlayerColor.None)
                    continue;
                pawns[othersColor].OnGot(other);
            }

            Debug.Log("PrepareManager Started");
        }

        //private void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.yellow;
        //    var pivot = othersStandSockets[2].position + new Vector3(0, -standPivotHeight, standPivotDepth);
        //    Gizmos.DrawWireSphere(pivot, 0.01f);
        //    Gizmos.color = Color.magenta;
        //    foreach(var socket in othersStandSockets)
        //    {
        //        Gizmos.DrawWireSphere(socket.position, 0.01f);
        //    }
        //    Gizmos.DrawLine(pivot, pivot + Quaternion.Euler(-Mathf.Atan2(standPivotHeight, -standPivotDepth) / Mathf.PI * 180, 0, 0) * new Vector3(0, 0, Mathf.Sqrt(standPivotHeight * standPivotHeight + standPivotDepth * standPivotDepth)));
        //}

        #endregion

        #region Photon Callbacks

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player other)
        {
            Debug.Log("OnPlayerEnteredRoom() " + other.NickName + "(" + other.ActorNumber + ")"); // not seen if you're the player connecting
            for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount - 2; i++)
            {
                MoveStand(PhotonNetwork.PlayerListOthers[i].ActorNumber, i);
            }
            SpawnStand(other, PhotonNetwork.CurrentRoom.PlayerCount - 2);
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.Log("OnPlayerLeftRoom() " + other.NickName + "(" + other.ActorNumber + ")"); // seen when other disconnects
            var othersColor = (PlayerColor)other.CustomProperties[PlayerManager.ColorKey];
            if (othersColor != PlayerColor.None)
            {
                pawns[othersColor].OnReturned(false);
            }
            var stand = otherStands[other.ActorNumber];
            var socketPos = stand.transform.position;
            var socketDir = socketPos;
            socketDir.Scale(new Vector3(1, 0, 1));
            socketDir.Normalize();
            var pivotPos = socketPos + socketDir * standPivotDepth - new Vector3(0, standPivotHeight, 0);
            var socketAngle = Mathf.Atan2(socketDir.x, socketDir.z);
            DOTween.To(
                () => pivotToStandAngle,
                (x) => stand.transform.position = pivotPos + Quaternion.Euler(-x * Mathf.Rad2Deg, 0, 0) * Quaternion.Euler(0, socketAngle, 0) * new Vector3(0, 0, pivotToStandDistance),
                0,
                standAppearTime).
                SetEase(Ease.Linear).
                OnComplete(() => Destroy(stand.gameObject));
            otherStands.Remove(other.ActorNumber);
            for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount - 1; i++)
            {
                MoveStand(PhotonNetwork.PlayerListOthers[i].ActorNumber, i);
            }
        }

        public override void OnLeftRoom()
        {
            Debug.Log("OnLeftRoom()");
            Destroy(PlayerManager.LocalPlayerInstance);
            SceneManager.LoadScene("Launcher");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("OnDisconnected()");
            var localPlayerColor = (PlayerColor)PhotonNetwork.LocalPlayer.CustomProperties[PlayerManager.ColorKey];
            if (localPlayerColor != PlayerColor.None)
            {
                pawns[localPlayerColor].photonView.RPC("OnReturned", RpcTarget.Others, false);
            }
        }

        #endregion

        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void QuitApplication()
        {
            Application.Quit();
        }

        public Transform GetStandSocket(int index)
        {
            switch (PhotonNetwork.CurrentRoom.PlayerCount)
            {
                case 2:
                    return othersStandSockets[2];
                case 3:
                    return othersStandSockets[1 + index * 2];
                case 4:
                    return othersStandSockets[index * 2];
                default:
                    return null;
            }
        }

        public PawnStand GetStand(int actorNumber)
        {
            if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                return myStand;
            return otherStands[actorNumber];
        }

        #endregion

        #region Private Methods

        PawnStand SpawnStand(Player owner, int standIndex)
        {
            var socketTransform = GetStandSocket(standIndex);
            var socketDir = socketTransform.position;
            socketDir.Scale(new Vector3(1, 0, 1));
            socketDir.Normalize();
            var pivotPos = socketTransform.position + socketDir * standPivotDepth - new Vector3(0, standPivotHeight, 0);
            var socketAngle = Mathf.Atan2(socketDir.x, socketDir.z);
            var stand = Instantiate(pawnStandPrefab, pivotPos + socketDir * pivotToStandDistance, Quaternion.identity).GetComponent<PawnStand>();
            DOTween.To(
                () => 0f,
                (x) => stand.transform.position = pivotPos + Quaternion.Euler(-x * Mathf.Rad2Deg, 0, 0) * Quaternion.Euler(0, socketAngle, 0) * new Vector3(0, 0, pivotToStandDistance),
                pivotToStandAngle,
                standAppearTime).
                SetEase(Ease.Linear);
            otherStands.Add(owner.ActorNumber, stand);
            stand.SetNameText(owner.NickName);
            return stand;
        }

        void MoveStand(int actorNumber, int standIndex)
        {
            otherStands[actorNumber].MoveTo(GetStandSocket(standIndex).position);
        }

        public void OnBellClicked()
        {
            var players = PhotonNetwork.PlayerList;
            if (players.Length < 2)
                return;
            foreach (var player in players)
                if ((PlayerColor)player.CustomProperties[PlayerManager.ColorKey] == PlayerColor.None)
                    return;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            var se = fade.FadeOut();
            if (!PhotonNetwork.IsMasterClient)
                return;
            se.OnComplete(() => PhotonNetwork.LoadLevel("Game"));
        }

        #endregion

    }
}