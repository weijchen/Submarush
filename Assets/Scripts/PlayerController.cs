using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Valve.VR;

namespace Team73.Round5.Racing
{
    public enum PlayerOpt : int
    {
        P1 = 1, 
        P2 = 2,
        P3 = 3, 
        P4 = 4
    }
    
    public class PlayerController : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private GameObject controller;
        [SerializeField] private Transform spinPoint;
        [SerializeField] private PlayerOpt playerOpt = PlayerOpt.P1;
        [SerializeField] private UIManager uIManager;
        [SerializeField] private float punishTime = 2.0f;

        [Header("VFX")]
        [SerializeField] private ParticleSystem hitParticle;
        [SerializeField] private ParticleSystem LeftEngineParticleOne;
        [SerializeField] private ParticleSystem LeftEngineParticleTwo;
        [SerializeField] private ParticleSystem RightEngineParticleOne;
        [SerializeField] private ParticleSystem RightEngineParticleTwo;

        [Header("Info Related")] 
        [SerializeField] private ProgressBar _progressBarSelf;
        [SerializeField] private ProgressBar _progressBarOther;
        [SerializeField] private EnergyBar _energyBarSelf;
        [SerializeField] private EnergyBar _energyBarOther;

        private Vector3 moveInputVal = Vector3.zero;
        
        private Rigidbody _rigidbody;
        private AudioSource _audioSource;
        private Vector3 trackerPosition;
        private float initTrackLPosX;
        private float initTrackLPosY;
        private float initTrackLPosZ;
        private float initTrackRPosX;
        private float initTrackRPosY;
        private float initTrackRPosZ;

        private float xThrow;
        private float yThrow;
        private bool isCalibrating = true;
        private bool isPunished = false;
        private float timer = 0;
        private int energyCollected = 0;
        private int currProgress = 0;
        private bool canMove = true;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _audioSource = GetComponent<AudioSource>();

            if (GameManager.Instance.useTracker)
            {
                trackerPosition = controller.transform.position;
            }
            
            Calibration();
        }
        
        void Update()
        {
            GetTrackerTransform();
            if (!isCalibrating)
            {
                if (canMove)
                {
                    AddDirectionalForce();
                    AddForwardForce();
                    ProcessRotation();
                    ControlDrag();    
                }
            }

            if (isPunished)
            {
                if (timer <= punishTime)
                {
                    timer += Time.deltaTime;
                }
                else
                {
                    timer = 0f;
                    isPunished = false;
                }
            }
        }
        
        private void FixedUpdate()
        {
            _rigidbody.AddForce(moveInputVal, ForceMode.Acceleration);
            
            // Set Speed Maximum Limit
            if (_rigidbody.velocity.magnitude > GameManager.Instance.maxDirSpeed)
            { 
                _rigidbody.velocity = _rigidbody.velocity.normalized * GameManager.Instance.maxDirSpeed * Time.deltaTime;
            }
        }

        void Calibration()
        {
            StartCoroutine(StartCalibration(GameManager.Instance.calibrationSmoothness, GameManager.Instance.calibrationDuration));
        }

        IEnumerator StartCalibration(float smoothness, float duration)
        {
            float progress = 0f;
            string calibrateString;
            List<float> xList = new List<float>();
            List<float> yList = new List<float>();
            List<float> zList = new List<float>();
            
            while (progress <= duration)
            {
                progress += smoothness;
                if (GameManager.Instance.useTracker)
                {
                    xList.Add(trackerPosition.x);
                    yList.Add(trackerPosition.y);
                    zList.Add(trackerPosition.z);
                }
                if (progress <= 1)
                {
                    calibrateString = "Ready";
                }
                else if (progress >= duration-1)
                {
                    calibrateString = "Go!";
                }
                else
                {
                    calibrateString = Mathf.RoundToInt(duration - progress).ToString();
                }
                uIManager.SetCalibrateWord(calibrateString);
                yield return new WaitForSeconds(smoothness);
            }

            if (GameManager.Instance.useTracker)
            {
                initTrackLPosX = xList.Average();
                initTrackLPosY = yList.Average();
                initTrackLPosZ = zList.Average();
            }
            
            LeftEngineParticleOne.Play();
            LeftEngineParticleTwo.Play();
            RightEngineParticleOne.Play();
            RightEngineParticleTwo.Play();
            isCalibrating = false;
            uIManager.FinishCalibrate();
        }

        void GetTrackerTransform()
        {
            trackerPosition = controller.transform.position;
        }
        
        private void AddDirectionalForce()
        {
            float verticalForce;
            float horizontalForce;
            
            if (GameManager.Instance.useTracker)
            {
                horizontalForce = trackerPosition.z - initTrackLPosZ;
                //Debug.LogFormat("Horizontal Force: {0}", horizontalDiff);
                
                if (horizontalForce >= GameManager.Instance.horizontalThreshold)
                {
                    horizontalForce = -GameManager.Instance.horizontalForceMulti;
                }
                else if (horizontalForce <= -GameManager.Instance.horizontalThreshold)
                {
                    horizontalForce = GameManager.Instance.horizontalForceMulti;
                } 
                else
                {
                    horizontalForce = 0f;
                }

                verticalForce = trackerPosition.y - initTrackLPosY;
                //Debug.LogFormat("Vertical Force: {0}", vertForce);

                if (verticalForce >= GameManager.Instance.verticalThreshold)
                {
                    verticalForce = GameManager.Instance.verticalForceMulti;
                } 
                else if (verticalForce <= -GameManager.Instance.verticalThreshold)
                {
                    verticalForce = -GameManager.Instance.verticalForceMulti;
                } 
                else
                {
                    verticalForce = 0f;
                }
                
                moveInputVal = verticalForce * transform.up + horizontalForce * transform.right;
            }
            else
            {
                if (playerOpt == PlayerOpt.P1)
                {
                    verticalForce = Input.GetAxisRaw("Vertical") * GameManager.Instance.verticalForceMulti;
                    horizontalForce = Input.GetAxisRaw("Horizontal") * GameManager.Instance.horizontalForceMulti;
                    moveInputVal = verticalForce * transform.up + horizontalForce * transform.right;
                } 
                // else if (playerOpt == PlayerOpt.P2)
                else
                {
                    verticalForce = Input.GetAxisRaw("Vertical_2") * GameManager.Instance.verticalForceMulti;
                    horizontalForce = Input.GetAxisRaw("Horizontal_2") * GameManager.Instance.horizontalForceMulti;
                    moveInputVal = verticalForce * transform.up + horizontalForce * transform.right;
                }
                // else if (playerOpt == PlayerOpt.P3)
                // {
                //     verticalForce = Input.GetAxisRaw("Vertical") * GameManager.Instance.verticalForceMulti;
                //     horizontalForce = Input.GetAxisRaw("Horizontal") * GameManager.Instance.horizontalForceMulti;
                //     moveInputVal = verticalForce * transform.up + horizontalForce * transform.right;
                // }
                // else if (playerOpt == PlayerOpt.P4)
                // {
                //     verticalForce = Input.GetAxisRaw("Vertical") * GameManager.Instance.verticalForceMulti;
                //     horizontalForce = Input.GetAxisRaw("Horizontal") * GameManager.Instance.horizontalForceMulti;
                //     moveInputVal = verticalForce * transform.up + horizontalForce * transform.right;
                // }
            }

            xThrow = horizontalForce;
            yThrow = verticalForce;
        }
        
        private void AddForwardForce()
        {
            bool hasForwardForce;
            if (GameManager.Instance.useTracker)
            {
                if (playerOpt == PlayerOpt.P1)
                {
                    hasForwardForce = Input.GetKey(KeyCode.A);
                } else
                {
                    hasForwardForce = Input.GetKey(KeyCode.L);
                }
                /*
                Debug.LogFormat("(L) Forward force: {0}", leftForwardForce);
                Debug.LogFormat("(R) Forward force: {0}", rightForwardForce);
                Debug.LogFormat("(M) Forward force: {0}", forwardForce);
                */

                if (hasForwardForce)
                {
                    if (isPunished)
                    {
                        moveInputVal += transform.forward * (GameManager.Instance.forwardForceMulti / 10);
                    }
                    else
                    {
                        moveInputVal += transform.forward * GameManager.Instance.forwardForceMulti;
                    }
                }
            }
            else
            {
                if (playerOpt == PlayerOpt.P1)
                {
                    hasForwardForce = Input.GetKey(KeyCode.LeftShift);
                }
                else
                {
                    hasForwardForce = Input.GetKey(KeyCode.RightShift);
                }
                    
                if (hasForwardForce)
                {
                    if (isPunished)
                    {
                        moveInputVal += transform.forward * (GameManager.Instance.forwardForceMulti / 10);
                    }
                    else
                    {
                        moveInputVal += transform.forward * GameManager.Instance.forwardForceMulti;
                    }
                }
            }
        }
        
        private void ProcessRotation()
        {
            float yaw = xThrow * GameManager.Instance.controlYawFactor;
            // float pitch = yThrow * controlPitchFactor;
            transform.RotateAround(spinPoint.position, Vector3.up, yaw * GameManager.Instance.rotationMulti);
        }

        private void ControlDrag()
        {
            _rigidbody.drag = GameManager.Instance.drag;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.transform.CompareTag("Obstacles"))
            {
                SoundManager.Instance.PlaySFX(GameManager.Instance.hitClip);
                hitParticle.Play();
            }
        }

        public void Punish()
        {
            isPunished = true;
            SoundManager.Instance.PlaySFX(GameManager.Instance.hitClip);
        }

        public void CollectEnergy()
        {
            _energyBarSelf.EnableEnergyOnGrid(energyCollected);
            _energyBarOther.EnableEnergyOnGrid(energyCollected);
            energyCollected += 1;
            SoundManager.Instance.PlayCollectSFX(energyCollected-1);
        }

        public int GetCollectEnergy()
        {
            return energyCollected;
        }

        public void AdvanceProgress()
        {
            _progressBarSelf.EnableProgressOnGrid(currProgress);
            _progressBarOther.EnableProgressOnGrid(currProgress);
            currProgress += 1;
        }

        public void DestroySelf()
        {
            SoundManager.Instance.PlaySFX(GameManager.Instance.hitClip);
            GetComponent<Rigidbody>().useGravity = true;
            canMove = false;
        }
    }
}
