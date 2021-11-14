using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Team73.Round5.Racing
{
    public enum SFXList : int
    {
        CarDrive = 0,
        Door = 1,
    }
    
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance = null;

        [SerializeField] private AudioClip[] bgmList;
        [SerializeField] private AudioClip[] sfxList;
        [SerializeField] private AudioClip[] collectList;
        
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

        public void PlaySFXByIndex(SFXList sfxIndex, float volume = 1.0f)
        {
            _audioSource.PlayOneShot(sfxList[(int)sfxIndex], volume);
        }

        public void PlaySFX(AudioClip audioClip, float volume = 1.0f)
        {
            _audioSource.PlayOneShot(audioClip, volume);
        }

        public void PlayCollectSFX(int index)
        {
            if (index >= collectList.Length)
            {
                index = collectList.Length - 1;
            }

            _audioSource.PlayOneShot(collectList[index]);
        }
    }
}
