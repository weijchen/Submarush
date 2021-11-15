using System;
using System.Collections;
using System.Collections.Generic;
using Team73.Round5.Racing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartCanvas : MonoBehaviour
{
    [SerializeField] private bool isTutorial = false;
    [SerializeField] private float timeToChg = 5.0f;

    private float timer = 0f;

    private void Update()
    {
        if (isTutorial)
        {
            if (timer < timeToChg)
            {
                timer += Time.deltaTime;
            }
            else
            {
                SoundManager.Instance.StopPlay();
                SoundManager.Instance.PlayBGM(1);
                SceneManager.LoadScene(2);
            }
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void EndGame()
    {
        Application.Quit();
    }
}
