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

        [Header("VFX")]
        [SerializeField] private ParticleSystem hitParticle;
        [SerializeField] private ParticleSystem LeftEngineParticleOne;
        [SerializeField] private ParticleSystem LeftEngineParticleTwo;
        [SerializeField] private ParticleSystem RightEngineParticleOne;
        [SerializeField] private ParticleSystem RightEngineParticleTwo;

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

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _audioSource = GetComponent<AudioSource>();

            if (GameManager.Instance.useTracker)
            {
                trackerPosition = controller.transform.position;
                Calibration();
            }
            else
            {
                isCalibrating = false;
                LeftEngineParticleOne.Play();
                LeftEngineParticleTwo.Play();
                RightEngineParticleOne.Play();
                RightEngineParticleTwo.Play();
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
            List<float> xListL = new List<float>();
            List<float> yListL = new List<float>();
            List<float> zListL = new List<float>();
            List<float> xListR = new List<float>();
            List<float> yListR = new List<float>();
            List<float> zListR = new List<float>();
            while (progress <= duration)
            {
                progress += smoothness;
                xListL.Add(trackerPosition.x);
                yListL.Add(trackerPosition.y);
                zListL.Add(trackerPosition.z);
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
                UIManager.Instance.SetCalibrateWord(calibrateString);
                yield return new WaitForSeconds(smoothness);
            }
            initTrackLPosX = xListL.Average();
            initTrackLPosY = yListL.Average();
            initTrackLPosZ = zListL.Average();
            initTrackRPosX = xListR.Average();
            initTrackRPosY = yListR.Average();
            initTrackRPosZ = zListR.Average();
            isCalibrating = false;
            UIManager.Instance.FinishCalibrate();
            LeftEngineParticleOne.Play();
            LeftEngineParticleTwo.Play();
            RightEngineParticleOne.Play();
            RightEngineParticleTwo.Play();
        }

        void Update()
        {
            GetTrackerTransform();
            if (!isCalibrating)
            {
                AddDirectionalForce();
                AddForwardForce();
                ProcessRotation();
                ControlDrag();
            }
        }

        void GetTrackerTransform()
        {
            trackerPosition = controller.transform.position;
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
            if (GameManager.Instance.useTracker)
            {
                bool hasForwardForce = Input.GetKey(KeyCode.LeftShift);
                bool hasBackwardForce = Input.GetKey(KeyCode.LeftControl);
                /*
                Debug.LogFormat("(L) Forward force: {0}", leftForwardForce);
                Debug.LogFormat("(R) Forward force: {0}", rightForwardForce);
                Debug.LogFormat("(M) Forward force: {0}", forwardForce);
                */

                if (hasForwardForce)
                {
                    moveInputVal += transform.forward * GameManager.Instance.forwardForceMulti;
                }
                if (hasBackwardForce)
                {
                    moveInputVal += transform.forward * -GameManager.Instance.forwardForceMulti;
                
                    // Brake
                    if (moveInputVal.z <= 0)
                    {
                        moveInputVal.z = 0;
                    }
                }
            }
            else
            {
                bool hasForwardForce;
                bool hasBackwardForce;
                if (playerOpt == PlayerOpt.P1)
                {
                    hasForwardForce = Input.GetKey(KeyCode.LeftShift);
                    hasBackwardForce = Input.GetKey(KeyCode.LeftControl);
                }
                else
                {
                    hasForwardForce = Input.GetKey(KeyCode.RightShift);
                    hasBackwardForce = Input.GetKey(KeyCode.RightControl);
                }
                    
                if (hasForwardForce)
                {
                    moveInputVal += transform.forward * GameManager.Instance.forwardForceMulti;
                }
                if (hasBackwardForce)
                {
                    moveInputVal += transform.forward * -GameManager.Instance.forwardForceMulti;
                
                    // Brake
                    if (moveInputVal.z <= 0)
                    {
                        moveInputVal.z = 0;
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
    }
}

