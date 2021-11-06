using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
// using UnityEngine.UI;
using Photon.Pun;

namespace Team73.Round5.Racing
{
    public class PlayerNameInputField : MonoBehaviour
    {
        const string playerNamePrefKey = "PlayerName";

        private void Start()
        {
            string defaultName = string.Empty;
            TMP_InputField _inputField = GetComponent<TMP_InputField>();
            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName;
        }

        public void SetPlayerName()
        {
            string value = GetComponent<TMP_InputField>().text;
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }

            PhotonNetwork.NickName = value;
            PlayerPrefs.SetString(playerNamePrefKey, value);
        }
    }
}
