using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static GameObject LocalPlayerInstance;
    
    [Tooltip("GameObject PlayerUI")]
    [SerializeField] public GameObject PlayerUIPrefab;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        AdjustCameraWork();
        InstantiatePlayerUI();
        
        #if UNITY_5_4_OR_NEWER
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += 
            (scene, loadingMode) =>
            {
                if (this != null)
                {
                    CalledOnLevelWasLoaded(scene.buildIndex);
                }
            };
        #endif
    }

    private void AdjustCameraWork()
    {
        
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
}
