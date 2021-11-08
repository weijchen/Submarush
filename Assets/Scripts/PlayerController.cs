using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Photon.Pun;
using UnityEngine;
using Valve.VR;

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
        [SerializeField] private Transform rotationBody;
        [SerializeField] private float controlRollFactor = -20.0f;
        
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
            StartCoroutine(StartCalibration(0.01f, 2.0f));
        }

        IEnumerator StartCalibration(float smoothness, float duration)
        {
            float progress = 0f;
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
                yield return new WaitForSeconds(smoothness);
            }
            initTrackLPosX = xListL.Average();
            initTrackLPosY = yListL.Average();
            initTrackLPosZ = zListL.Average();
            initTrackRPosX = xListR.Average();
            initTrackRPosY = yListR.Average();
            initTrackRPosZ = zListR.Average();
            isCalibrating = false;
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
                    // ProcessRotation();
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
                Debug.Log(Input.GetAxisRaw("Vertical"));
                Debug.Log(Input.GetAxisRaw("Horizontal"));
                moveInputVal = verticalForce * transform.up + horizontalForce * transform.right;
            }
            else
            {
                verticalForce = Input.GetAxisRaw("Vertical") * verticalForceMulti;
                horizontalForce = Input.GetAxisRaw("Horizontal") * horizontalForceMulti;

                moveInputVal = verticalForce * transform.up + horizontalForce * transform.right;
            }
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
        
        // TODO: meca rotation will be done after having the meca model
        // private void ProcessRotation()
        // {
        //     float row = xThrow * controlRollFactor;
        //     rotationBody.localRotation = Quaternion.Euler(row,-90.0f, 0);
        //     transform.Rotate(0.0f, Input.GetAxisRaw("Horizontal") * rotationMulti, 0.0f);
        // }

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

