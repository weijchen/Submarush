using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] public PlayerOpt playerOpt = PlayerOpt.P1;
        [SerializeField] private UIManager uIManager;
        [SerializeField] private float punishTime = 2.0f;
        [SerializeField] private Image winImage;
        [SerializeField] private Image loseImage;

        [Header("VFX")]
        [SerializeField] private ParticleSystem hitParticle;
        [SerializeField] private ParticleSystem LEngineParticleOneBlast;
        [SerializeField] private ParticleSystem LEngineParticleOneBubble;
        [SerializeField] private ParticleSystem LEngineParticleTwoBlast;
        [SerializeField] private ParticleSystem LEngineParticleTwoBubble;
        [SerializeField] private ParticleSystem REngineParticleOneBlast;
        [SerializeField] private ParticleSystem REngineParticleOneBubble;
        [SerializeField] private ParticleSystem REngineParticleTwoBlast;
        [SerializeField] private ParticleSystem REngineParticleTwoBubble;
        [SerializeField] private ParticleSystem laserImpactedVFX;
        [SerializeField] private ParticleSystem orbCollectVFX;
        [SerializeField] private ParticleSystem[] explositonVFX;
        [SerializeField] private GameObject shield;
        [SerializeField] private ParticleSystem shieldBreakVFX;

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
        private bool hasPlayThree = false;
        private bool hasPlayTwo = false;
        private bool hasPlayOne = false;
        private bool hasPlayGo = false;
        private bool showEnding = false;
        private bool isWinner = false;
        public bool isShieldOpen = false;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _audioSource = GetComponent<AudioSource>();

            if (GameManager.Instance.useTracker)
            {
                trackerPosition = controller.transform.position;
            }
            
            shield.SetActive(false);
            winImage.enabled = true;
            loseImage.enabled = true;
            Calibration();
            // StartCoroutine(StartEnding(winImage, 0.01f, 2f));
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
                    LEngineParticleOneBlast.Play();
                    LEngineParticleOneBubble.Play();
                    LEngineParticleTwoBlast.Play();
                    LEngineParticleTwoBubble.Play();
                    REngineParticleOneBlast.Play();
                    REngineParticleOneBubble.Play();
                    REngineParticleTwoBlast.Play();
                    REngineParticleTwoBubble.Play();
                    GetComponent<MeshRenderer>().material.color = Color.white;
                }
            }
        }

        IEnumerator StartEnding(Image i, float smoothness, float duration)
        {
            float progress = 0;
            // float step = smoothness / duration;
            i.material.color = new Color(i.material.color.r, i.material.color.g, i.material.color.b, 0);
            while (progress < duration)
            {
                i.material.color = Color.Lerp(i.material.color, new Color(i.material.color.r, i.material.color.g, i.material.color.b, 255), progress / duration);
                progress += Time.deltaTime;
                yield return new WaitForSeconds(smoothness);
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
                progress += Time.deltaTime;
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
                else if (progress > duration-1)
                {
                    
                    calibrateString = "Go!";
                }
                else
                {
                    if (progress > 0.8f && !hasPlayOne)
                    {
                        hasPlayOne = true;
                        SoundManager.Instance.PlaySFX(SoundManager.Instance.sfxList[(int)SFXList.Countdown]);
                    }
                    
                    if (progress > 1.8f && !hasPlayTwo)
                    {
                        hasPlayTwo = true;
                        SoundManager.Instance.PlaySFX(SoundManager.Instance.sfxList[(int)SFXList.Countdown]);
                    }
                    
                    if (progress > 2.8f && !hasPlayThree)
                    {
                        hasPlayThree = true;
                        SoundManager.Instance.PlaySFX(SoundManager.Instance.sfxList[(int)SFXList.Countdown]);
                    }
                    
                    calibrateString = Mathf.RoundToInt(duration - progress).ToString();
                }
                uIManager.SetCalibrateWord(calibrateString);
                yield return null;
            }

            if (GameManager.Instance.useTracker)
            {
                initTrackLPosX = xList.Average();
                initTrackLPosY = yList.Average();
                initTrackLPosZ = zList.Average();
            }
            
            LEngineParticleOneBlast.Play();
            LEngineParticleOneBubble.Play();
            LEngineParticleTwoBlast.Play();
            LEngineParticleTwoBubble.Play();
            REngineParticleOneBlast.Play();
            REngineParticleOneBubble.Play();
            REngineParticleTwoBlast.Play();
            REngineParticleTwoBubble.Play();
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

                verticalForce = trackerPosition.x - initTrackLPosX;
                //Debug.LogFormat("Vertical Force: {0}", vertForce);

                if (verticalForce >= GameManager.Instance.verticalThreshold)
                {
                    verticalForce = -GameManager.Instance.verticalForceMulti;
                } 
                else if (verticalForce <= -GameManager.Instance.verticalThreshold)
                {
                    verticalForce = GameManager.Instance.verticalForceMulti;
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
                    hasForwardForce = Input.GetKey(KeyCode.RightControl);
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
            LEngineParticleOneBlast.Stop();
            LEngineParticleOneBubble.Stop();
            LEngineParticleTwoBlast.Stop();
            LEngineParticleTwoBubble.Stop();
            REngineParticleOneBlast.Stop();
            REngineParticleOneBubble.Stop();
            REngineParticleTwoBlast.Stop();
            REngineParticleTwoBubble.Stop();
            laserImpactedVFX.Play();
            GetComponent<MeshRenderer>().material.color = Color.red;
        }

        public void CollectEnergy()
        {
            _energyBarSelf.EnableEnergyOnGrid(energyCollected);
            _energyBarOther.EnableEnergyOnGrid(energyCollected);
            energyCollected += 1;
            SoundManager.Instance.PlayCollectSFX(energyCollected);
            if (energyCollected == GameManager.Instance.energyToPass)
            {
                isShieldOpen = true;
                SoundManager.Instance.PlaySFX(SoundManager.Instance.sfxList[(int)SFXList.Shield]);
                shield.SetActive(true);
            }
            orbCollectVFX.Play();
        }

        public void BreakShield()
        {
            shieldBreakVFX.Play();
            shield.SetActive(false);
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
            foreach (ParticleSystem system in explositonVFX)
            {
                system.Play();
            }
            SoundManager.Instance.PlaySFX(GameManager.Instance.hitClip);
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularDrag = 1.0f;
            canMove = false;
            LEngineParticleOneBlast.Stop();
            LEngineParticleOneBubble.Stop();
            LEngineParticleTwoBlast.Stop();
            LEngineParticleTwoBubble.Stop();
            REngineParticleOneBlast.Stop();
            REngineParticleOneBubble.Stop();
            REngineParticleTwoBlast.Stop();
            REngineParticleTwoBubble.Stop();
            loseImage.enabled = true;
            StartCoroutine(StartEnding(loseImage, 0.01f, 2f));
        }

        public void Victory()
        {
            winImage.enabled = true;
            StartCoroutine(StartEnding(winImage, 0.01f, 2f));
        }
    }
}
