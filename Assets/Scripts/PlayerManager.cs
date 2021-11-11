using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Tooltip("GameObject PlayerUI")]
    [SerializeField] public GameObject PlayerUIPrefab;
    
    private void Awake()
    {
        
    }
    
    private void Start()
    {
        // InstantiatePlayerUI();
    }

    // private void InstantiatePlayerUI()
    // {
    //     if (PlayerUIPrefab != null)
    //     {
    //        
    //         GameObject _uiGo = Instantiate(PlayerUIPrefab);
    //         _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);    
    //     }
    //     else
    //     {
    //         Debug.LogWarning("GameObject PlayerUI is null", this);
    //     }
    // }
}
