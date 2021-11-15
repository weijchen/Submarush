using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBar : MonoBehaviour
{
    [SerializeField] private GameObject[] energyGrid;
    
    private void Start()
    {
        foreach (GameObject o in energyGrid)
        {
            o.SetActive(false);
        }
    }

    public void EnableEnergyOnGrid(int gridIndex)
    {
        energyGrid[gridIndex].SetActive(true);
    }
}
