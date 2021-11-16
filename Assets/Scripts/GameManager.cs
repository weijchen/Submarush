using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace Team73.Round5.Racing
{
    public enum SceneIndex : int
    {
        Start = 0,
        Tutorial = 1,
        Story = 2,
        Main = 3,
        End = 4
    }
    
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance = null;

        [Header("General")]
        [SerializeField] public bool useTracker = false;
        [SerializeField] public AudioClip hitClip;
        [SerializeField] public float calibrationSmoothness = 0.01f;
        [SerializeField] public float calibrationDuration = 4.0f;
        [SerializeField] public int energyToPass = 10;
        
        [Header("Movement")]
        [SerializeField] public float verticalForceMulti = 10.0f;
        [SerializeField] public float horizontalForceMulti = 5.0f;
        [SerializeField] public float maxDirSpeed = 100.0f;
        [SerializeField] public float forwardForceMulti = 10.0f;
        [SerializeField] public float drag = 2.0f;
        [SerializeField] public float rotationMulti = 0.03f;
        [SerializeField] public float verticalThreshold = 0.15f;
        [SerializeField] public float horizontalThreshold = 0.25f;
        [SerializeField] public float forwardThreshold = 0.25f;

        [Header("Rotation")]
        [SerializeField] public float controlYawFactor = 2.0f;
        [SerializeField] public float controlPitchFactor = 2.0f;

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
    }
}
