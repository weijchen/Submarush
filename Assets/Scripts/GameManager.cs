using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Debug = UnityEngine.Debug;

namespace Team73.Round5.Racing
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager Instance = null;

        [Tooltip("Prefab")] 
        public GameObject playerPrefab;

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
        }

        private void Start()
        {
            if (playerPrefab == null)
            {
                Debug.LogError("playerPrefab is missing, please set in Game Manager");
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("Instantiate player prefab {0}", SceneManager.GetActiveScene().name);
                    PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);

                    // if (!player.GetPhotonView().IsMine)
                        // return;
                    // player.transform.Find("Meca").transform.Find("Camera").gameObject.GetComponent<CameraController>().enabled = true;
                    // player.transform.Find("Meca").transform.Find("Camera").gameObject.GetComponent<CameraController>().SetTarget(player.transform.Find("Meca").transform);
                    // player.transform.Find("Meca").transform.Find("Camera").gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogFormat("Ignore scene loading for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
        }

        public override void OnLeftRoom() {
            SceneManager.LoadScene(0);
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
        
        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("{0} enter the room", other.NickName);

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("Am I Master Client? {0}", PhotonNetwork.IsMasterClient);
                LoadArena();
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("{0} leave the room", other.NickName);

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("Am I Master Client? {0}", PhotonNetwork.IsMasterClient);
                LoadArena();
            }
        }
        
        private void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("Not master client, no loading scene");
            }

            Debug.LogFormat("Loading scene for {0} people", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
        }
    }
}
