using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private GameObject[] progressGrid;
    
    private void Start()
    {
        foreach (GameObject o in progressGrid)
        {
            o.SetActive(false);
        }
    }

    public void EnableProgressOnGrid(int gridIndex)
    {
        progressGrid[gridIndex].SetActive(true);
        if (gridIndex > 0)
        {
            progressGrid[gridIndex-1].transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
