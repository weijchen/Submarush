using System;
using System.Collections;
using System.Collections.Generic;
using Team73.Round5.Racing;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().Punish();
        }
    }
}
