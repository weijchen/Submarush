using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Team73.Round5.Racing
{
    
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance = null;

        [SerializeField] private AudioClip[] bgmList;
        [SerializeField] private AudioClip[] sfxList;
        
        private int curScene = 0;
        private bool isBGMPlaying = false;
        
        private AudioSource _audioSource;

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
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            PlayBGM(0);
        }

        public void PlayBGM(int index)
        {
            _audioSource.clip = bgmList[index];
            _audioSource.Play();
        }

        public void StopPlay()
        {
            _audioSource.Stop();
        }

        public void PlaySFX(AudioClip audioClip, float volume = 1.0f)
        {
            _audioSource.PlayOneShot(audioClip, volume);
        }
    }
}
