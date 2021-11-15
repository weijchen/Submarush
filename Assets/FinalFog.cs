using System;
using System.Collections;
using System.Collections.Generic;
using Team73.Round5.Racing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalFog : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            SceneManager.LoadScene((int)SceneIndex.End);
        }
    }
}
