using System;
using System.Collections;
using System.Collections.Generic;
using Team73.Round5.Racing;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{ 
    private bool hasP1Reach = false;
    private bool hasP2Reach = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            if (!hasP1Reach && other.GetComponent<PlayerController>().playerOpt == PlayerOpt.P1)
            {
                other.GetComponent<PlayerController>().AdvanceProgress();
                hasP1Reach = true;
            }

            if (!hasP2Reach && other.GetComponent<PlayerController>().playerOpt == PlayerOpt.P2)
            {
                other.GetComponent<PlayerController>().AdvanceProgress();
                hasP2Reach = true;
            }
        }
    }
}
