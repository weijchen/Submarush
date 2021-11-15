using System;
using System.Collections;
using System.Collections.Generic;
using Team73.Round5.Racing;
using UnityEngine;

public class Energy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            if (!other.GetComponent<PlayerController>().isShieldOpen)
            {
                other.GetComponent<PlayerController>().CollectEnergy();
                gameObject.SetActive(false);
            }
        }
    }
}
