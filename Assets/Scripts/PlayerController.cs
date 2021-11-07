using System.Collections;
using System.Collections.Generic;
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

        [Header("Movement")]
        [SerializeField] private float verticalForceMulti = 10.0f;
        [SerializeField] private float horizontalForceMulti = 5.0f;
        [SerializeField] private float maxDirSpeed = 100.0f;
        [SerializeField] private float forwardForceMulti = 2.0f;
        [SerializeField] private float drag = 2.0f;
        [SerializeField] private float rotationMulti = 0.15f;
        
        [Header("Rotation")]
        [SerializeField] private Transform rotationBody;
        [SerializeField] private float controlRollFactor = -20.0f;
        
        private Vector3 moveInputVal = Vector3.zero;
        
        private Rigidbody _rigidbody;
        private AudioSource _audioSource;
        private PhotonView _photonView;
        private Vector3 trackerLPosition;
        private Vector3 trackerRPosition;
        
        private float xThrow;
        private float yThrow;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _audioSource = GetComponent<AudioSource>();
            _photonView = GetComponent<PhotonView>();
        }
        
        void Update()
        {
            if (_photonView.IsMine)
            {
                GetTrackerTransform();
                AddDirectionalForce();
                AddForwardForce();
                // ProcessRotation();
                ControlDrag();    
            }
        }

        void GetTrackerTransform()
        {
            trackerLPosition = leftController.transform.position;
            trackerRPosition = rightController.transform.position;
            // Debug.LogFormat("Left tracker position: {0}", trackerLPosition);
            // Debug.LogFormat("Right tracker position: {0}", trackerRPosition);
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
                float leftForwardForce = trackerLPosition.z;
                float rightForwardForce = trackerRPosition.z;
                horizontalForce = leftForwardForce > rightForwardForce ? horizontalForceMulti : -horizontalForceMulti;
                
                float leftVertForce = trackerLPosition.y;
                float rightVertForce = trackerRPosition.y;
                float vertForce = (leftVertForce + rightVertForce) / 2;

                verticalForce = vertForce >= 0 ? verticalForceMulti : -verticalForceMulti;
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
                float leftForwardForce = trackerLPosition.z;
                float rightForwardForce = trackerRPosition.z;
                float forwardForce = (leftForwardForce + rightForwardForce) / 2;

                if (forwardForce >= 0)
                {
                    moveInputVal += transform.forward * forwardForceMulti;
                }
                else
                {
                    moveInputVal += transform.forward * -forwardForceMulti;
                
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
    }
}

