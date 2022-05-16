using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace CantStop.Launcher
{
    [RequireComponent(typeof(InputField))]
    public class RoomNameInputField : MonoBehaviour
    {
        #region Private Constants

        const string roomNamePrefKey = "RoomName";

        #endregion

        #region Public Fields

        public static string RoomName 
        { 
            get { return PlayerPrefs.GetString(roomNamePrefKey); } 
            set { PlayerPrefs.SetString(roomNamePrefKey, value); }
        }

        #endregion

        #region MonoBehaviour CallBacks

        void Start()
        {

            InputField _inputField = this.GetComponent<InputField>();

            if (!PlayerPrefs.HasKey(roomNamePrefKey))
            {
                RoomName = "";
                return;
            }

            _inputField.text = RoomName;
        }

        #endregion

        #region Public Methods

        public void SetRoomName(string value)
        {
            RoomName = value;
        }

        #endregion
    }
}
