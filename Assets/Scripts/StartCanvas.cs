using System;
using System.Collections;
using System.Collections.Generic;
using Team73.Round5.Racing;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Team73.Round5.Racing
{
    public class StartCanvas : MonoBehaviour
    {
        [SerializeField] private bool isStory = false;
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
                    SceneManager.LoadScene((int)SceneIndex.Story);
                }
            }

            if (isStory)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    SoundManager.Instance.StopPlay();
                    SoundManager.Instance.PlayBGM(1);
                    SceneManager.LoadScene((int) SceneIndex.Main);
                }
            }
        }

        public void StartGame()
        {
            SceneManager.LoadScene((int)SceneIndex.Tutorial);
        }

        public void EndGame()
        {
            Application.Quit();
        }
    }
}
