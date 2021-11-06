using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Team73.Round5.Racing
{
    public class PhotonManager : MonoBehaviourPunCallbacks
    {
        public static PhotonManager Instance = null;

        [Tooltip("Player limit")]
        [SerializeField] private byte maxPlayersPerRoom = 4;

        [Tooltip("Display/Hide Player name and Play button")] 
        [SerializeField] private GameObject controlPanel;
        [Tooltip("Display/Hide Connecting debug")] 
        [SerializeField] private GameObject progressLabel;

        private string gameVersion = "1";
        private bool isConnecting;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                Instance = this;
            }
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Start()
        {
            controlPanel.SetActive(true);
            progressLabel.SetActive(false);
        }

        public void Connect()
        {
            isConnecting = true;
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
            controlPanel.SetActive(false);
            progressLabel.SetActive(true);
        }

        public override void OnConnectedToMaster()
        {
            if (isConnecting)
            {
                Debug.Log("PUN calls OnConnectedToMaster(), Photon Cloud connected.");
                PhotonNetwork.JoinRandomRoom();    
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN calls OnDisconnected() {0}.", cause);
            controlPanel.SetActive(true);
            progressLabel.SetActive(false);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN calls OnJoinRandomFailed(), fail to join random room");
            PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = maxPlayersPerRoom});
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN calls OnJoinedRoom(), join successfully!");

            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("I am the first one enter the room");
                PhotonNetwork.LoadLevel("Room for 1");
            }
        }
    }
}
