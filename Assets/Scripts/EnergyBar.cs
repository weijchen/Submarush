using System.Collections;
using System.Collections.Generic;
using Team73.Round5.Racing;
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
        if (gridIndex < GameManager.Instance.energyToPass)
            energyGrid[gridIndex].SetActive(true);
    }
}
