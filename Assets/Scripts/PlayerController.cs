using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Photon.Pun;
using UnityEngine;
using Valve.VR;
using TMPro;

namespace Team73.Round5.Racing
{
    public class PlayerController : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private bool useTracker = false;
        [SerializeField] private GameObject leftController;
        [SerializeField] private GameObject rightController;
        [SerializeField] private AudioClip runClip;
        [SerializeField] private AudioClip hitClip;
        [SerializeField] private float calibrationSmoothness = 0.01f;
        [SerializeField] private float calibrationDuration = 4.0f;
        
        [Header("VFX")]
        [SerializeField] private ParticleSystem hitParticle;
        [SerializeField] private ParticleSystem LeftEngineParticleOne;
        [SerializeField] private ParticleSystem LeftEngineParticleTwo;
        [SerializeField] private ParticleSystem RightEngineParticleOne;
        [SerializeField] private ParticleSystem RightEngineParticleTwo;

        [Header("Movement")]
        [SerializeField] private float verticalForceMulti = 10.0f;
        [SerializeField] private float horizontalForceMulti = 5.0f;
        [SerializeField] private float maxDirSpeed = 100.0f;
        [SerializeField] private float forwardForceMulti = 2.0f;
        [SerializeField] private float drag = 2.0f;
        [SerializeField] private float rotationMulti = 0.15f;
        [SerializeField] private float verticalThreshold = 0.15f;
        [SerializeField] private float horizontalThreshold = 0.25f;
        [SerializeField] private float forwardThreshold = 0.25f;

        [Header("Rotation")]
        [SerializeField] private float controlYawFactor = 2.0f;
        [SerializeField] private float controlPitchFactor = 2.0f;
        
        private Vector3 moveInputVal = Vector3.zero;
        
        private Rigidbody _rigidbody;
        private AudioSource _audioSource;
        private PhotonView _photonView;
        private Vector3 trackerLPosition;
        private Vector3 trackerRPosition;
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
            _photonView = GetComponent<PhotonView>();

            trackerLPosition = leftController.transform.position;
            trackerRPosition = rightController.transform.position;
            Calibration();
        }

        void Calibration()
        {
            StartCoroutine(StartCalibration(calibrationSmoothness, calibrationDuration));
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
                xListL.Add(trackerLPosition.x);
                yListL.Add(trackerLPosition.y);
                zListL.Add(trackerLPosition.z);
                xListR.Add(trackerRPosition.x);
                yListR.Add(trackerRPosition.y);
                zListR.Add(trackerRPosition.z);
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
            if (_photonView.IsMine)
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
        }

        void GetTrackerTransform()
        {
            trackerLPosition = leftController.transform.position;
            trackerRPosition = rightController.transform.position;
        }
        
        private void FixedUpdate()
        {
            _rigidbody.AddForce(moveInputVal, ForceMode.Acceleration);
            
            // Set Speed Maximum Limit
            if (_rigidbody.velocity.magnitude > maxDirSpeed)
            { 
                _rigidbody.velocity = _rigidbody.velocity.normalized * maxDirSpeed * Time.deltaTime;
            }
        }
        
        private void AddDirectionalForce()
        {
            float verticalForce;
            float horizontalForce;
            
            if (useTracker)
            {
                float leftForwardForce = trackerLPosition.z - initTrackLPosZ;
                float rightForwardForce = trackerRPosition.z - initTrackRPosZ;
                
                float horizontalDiff = leftForwardForce - rightForwardForce;
                //Debug.LogFormat("Horizontal Force: {0}", horizontalDiff);
                if (horizontalDiff >= horizontalThreshold)
                {
                    horizontalForce = -horizontalForceMulti;
                }
                else if (horizontalDiff <= -horizontalThreshold)
                {
                    horizontalForce = horizontalForceMulti;
                } 
                else
                {
                    horizontalForce = 0f;
                }

                float leftVertForce = trackerLPosition.y - initTrackLPosY;
                float rightVertForce = trackerRPosition.y - initTrackRPosY;
                float vertForce = (leftVertForce + rightVertForce) / 2;
                //Debug.LogFormat("Vertical Force: {0}", vertForce);

                if (vertForce >= verticalThreshold)
                {
                    verticalForce = verticalForceMulti;
                } 
                else if (vertForce <= -verticalThreshold)
                {
                    verticalForce = -verticalForceMulti;
                } 
                else
                {
                    verticalForce = 0f;
                }
                moveInputVal = verticalForce * transform.up + horizontalForce * transform.right;
            }
            else
            {
                verticalForce = Input.GetAxisRaw("Vertical") * verticalForceMulti;
                horizontalForce = Input.GetAxisRaw("Horizontal") * horizontalForceMulti;
                moveInputVal = verticalForce * transform.up + horizontalForce * transform.right;
            }
            xThrow = horizontalForce;
            yThrow = verticalForce;
        }
        
        private void AddForwardForce()
        {
            if (useTracker)
            {
                float leftForwardForce = trackerLPosition.z - initTrackLPosZ;
                float rightForwardForce = trackerRPosition.z - initTrackRPosZ;
                float forwardForce = (leftForwardForce + rightForwardForce) / 2;

                /*
                Debug.LogFormat("(L) Forward force: {0}", leftForwardForce);
                Debug.LogFormat("(R) Forward force: {0}", rightForwardForce);
                Debug.LogFormat("(M) Forward force: {0}", forwardForce);
                */

                if (forwardForce <= -forwardThreshold)
                {
                    moveInputVal += transform.forward * forwardForceMulti;
                }
                else
                {
                    if (leftForwardForce <= -forwardThreshold || rightForwardForce <= -forwardThreshold)
                    {
                        moveInputVal += transform.forward * forwardForceMulti / 2;
                    } 
                    else
                    {
                        moveInputVal += transform.forward * -forwardForceMulti;
                    }

                    // Brake
                    if (moveInputVal.z <= 0)
                    {
                        moveInputVal.z = 0;
                    }
                }
            }
            else
            {
                bool hasForwardForce = Input.GetKey(KeyCode.LeftShift);
                bool hasBackwardForce = Input.GetKey(KeyCode.LeftControl);
                if (hasForwardForce)
                {
                    moveInputVal += transform.forward * forwardForceMulti;
                }
                if (hasBackwardForce)
                {
                    moveInputVal += transform.forward * -forwardForceMulti;
                
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
            float yaw = xThrow * controlYawFactor;
            float pitch = yThrow * controlPitchFactor;
            transform.localRotation = Quaternion.Euler(-pitch,yaw, 0);
        }

        private void ControlDrag()
        {
            _rigidbody.drag = drag;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.transform.CompareTag("Obstacles"))
            {
                SoundManager.Instance.PlaySFX(hitClip);
                hitParticle.Play();
            }
        }
    }
}

