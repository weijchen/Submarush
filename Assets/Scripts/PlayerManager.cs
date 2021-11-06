using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Pun.Demo.PunBasics;

public class PlayerManager : MonoBehaviour
{
    public static GameObject LocalPlayerInstance;
    
    [Tooltip("指標- GameObject PlayerUI")]
    [SerializeField] public GameObject PlayerUIPrefab;
    
    private CameraWork _cameraWork;
    private PhotonView _photonView;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();

        if (_photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _cameraWork = GetComponent<CameraWork>();

        InstantiatePlayerUI();
        AdjustCameraWork();
        
        #if UNITY_5_4_OR_NEWER
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += 
            (scene, loadingMode) =>
            {
                if (this != null)
                {
                    this.CalledOnLevelWasLoaded(scene.buildIndex);
                }
            };
        #endif
    }

    private void AdjustCameraWork()
    {
        if (_cameraWork != null)
        {
            if (_photonView.IsMine)
            {
                _cameraWork.OnStartFollowing();
            }
        }
        else
        {
            Debug.LogError("playerPrefab - CameraWork component is missing", this);
        }
    }

    private void InstantiatePlayerUI()
    {
        if (PlayerUIPrefab != null)
        {
            GameObject _uiGo = Instantiate(PlayerUIPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("GameObject PlayerUI is null", this);
        }
    }
#if UNITY_5_4_OR_NEWER

    void OnLevelWasLoaded(int level)
    {
        CalledOnLevelWasLoaded(level);
    }
    #endif
    
    void CalledOnLevelWasLoaded(int level)
    {
        if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
        {
            transform.position = new Vector3(0f, 5f, 0f);
        }
        
        GameObject _uiGo = Instantiate(this.PlayerUIPrefab);
        _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
    }

    public PhotonView GetPhotonView()
    {
        return _photonView;
    }
}