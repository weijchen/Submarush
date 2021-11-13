using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBtn : MonoBehaviour
{
    [SerializeField] private DoorObj _doorObj;

    private void Start()
    {
        _doorObj.OpenDoor();
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Player"))
        {
            _doorObj.OpenDoor();
            gameObject.SetActive(false);
        }
    }
}
