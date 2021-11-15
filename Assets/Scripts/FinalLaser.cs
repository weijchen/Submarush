using System;
using System.Collections;
using System.Collections.Generic;
using Team73.Round5.Racing;
using UnityEngine;

public class FinalLaser : MonoBehaviour
{
    [SerializeField] private DoorObj finalDoor;
    [SerializeField] private GameObject finalDoorSymbol;

    private bool hasPass = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            int energy = other.gameObject.GetComponent<PlayerController>().GetCollectEnergy();
            if (energy >= GameManager.Instance.energyToPass && !hasPass)
            {
                Collider collider = other.GetComponent<Collider>();
                finalDoor.OpenDoor();
                collider.enabled = false;
                finalDoorSymbol.SetActive(false);
                hasPass = true;
                other.GetComponent<PlayerController>().BreakShield();
            }
            else
            {
                other.gameObject.GetComponent<PlayerController>().DestroySelf();
                if (energy >= GameManager.Instance.energyToPass)
                {
                    other.GetComponent<PlayerController>().BreakShield();
                }
            }
        }
    }
}
