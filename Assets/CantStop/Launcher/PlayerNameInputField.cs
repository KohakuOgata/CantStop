using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace CantStop.Launcher
{
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        #region Private Constants

        const string playerNamePrefKey = "PlayerName";

        #endregion

        #region MonoBehaviour CallBacks

        void Start()
        {

            InputField _inputField = this.GetComponent<InputField>();

            Debug.Log("IsConnected" + PhotonNetwork.IsConnected);

            if (PhotonNetwork.IsConnected)
            {
                _inputField.text = PhotonNetwork.NickName;
                return;
            }

            if (!PlayerPrefs.HasKey(playerNamePrefKey))
                return;

            var defaultName = PlayerPrefs.GetString(playerNamePrefKey);
            _inputField.text = defaultName;
            PhotonNetwork.NickName = defaultName;
        }

        #endregion

        #region Public Methods

        public void SetPlayerName(string value)
        {
            // #Important
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }
            PhotonNetwork.NickName = value;

            PlayerPrefs.SetString(playerNamePrefKey, value);
        }

        #endregion
    }
}
