using System.Collections;
using System.Collections.Generic;
using Team73.Round5.Racing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartCanvas : MonoBehaviour
{
    public void StartGame()
    {
        SoundManager.Instance.StopPlay();
        SoundManager.Instance.PlayBGM(1);
        SceneManager.LoadScene(1);
    }

    public void EndGame()
    {
        Application.Quit();
    }
}
