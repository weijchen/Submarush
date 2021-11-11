using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [SerializeField] private TMP_Text calibrateText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void SetCalibrateWord(string newText)
    {
        calibrateText.text = newText;
    }

    public void FinishCalibrate()
    {
        calibrateText.gameObject.SetActive(false);
    }
}
