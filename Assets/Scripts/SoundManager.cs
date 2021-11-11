using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Team73.Round5.Racing
{
    public enum SFXIndex : int
    {
        
    }
    
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance = null;

        [SerializeField] private AudioClip[] bgmList;
        [SerializeField] private AudioClip[] sfxList;

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

        public void PlaySFX(AudioClip audioClip, float volume = 1.0f)
        {
            _audioSource.PlayOneShot(audioClip, volume);
        }
    }
}
