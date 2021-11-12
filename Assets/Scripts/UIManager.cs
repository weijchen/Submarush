using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text calibrateText;

    public void SetCalibrateWord(string newText)
    {
        calibrateText.text = newText;
    }

    public void FinishCalibrate()
    {
        calibrateText.gameObject.SetActive(false);
    }
}
