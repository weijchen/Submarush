using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Team73.Round5.Racing
{
    public class PlayerController : MonoBehaviour
    {
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
        
        private float xThrow;
        private float yThrow;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _audioSource = GetComponent<AudioSource>();
        }
        
        void Update()
        {
            AddDirectionalForce();
            AddForwardForce();
            // ProcessRotation();
            ControlDrag();
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
            float verticalForce = Input.GetAxisRaw("Vertical") * verticalForceMulti;
            float horizontalForce = Input.GetAxisRaw("Horizontal") * horizontalForceMulti;

            moveInputVal = verticalForce * transform.up + horizontalForce * transform.right;
        }
        
        private void AddForwardForce()
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

