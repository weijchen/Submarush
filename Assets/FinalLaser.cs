using System;
using System.Collections;
using System.Collections.Generic;
using Team73.Round5.Racing;
using UnityEngine;

public class FinalLaser : MonoBehaviour
{
    [SerializeField] private int energyToPass = 10;
    [SerializeField] private DoorObj finalDoor;
    [SerializeField] private GameObject finalDoorSymbol;

    private bool hasPass = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            int energy = other.gameObject.GetComponent<PlayerController>().GetCollectEnergy();
            if (energy >= energyToPass && !hasPass)
            {
                Collider collider = other.GetComponent<Collider>();
                finalDoor.OpenDoor();
                collider.enabled = false;
                finalDoorSymbol.SetActive(false);
                hasPass = true;
            }
            else
            {
                other.gameObject.GetComponent<PlayerController>().DestroySelf();
            }
        }
    }
}
