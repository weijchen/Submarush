using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Team73.Round5.Racing
{
    public class PlayerUI : MonoBehaviour
    {
        [Tooltip("Player Name")] 
        [SerializeField] private TMP_Text playerNameText;
        
        [Tooltip("Player HP")] 
        [SerializeField] private Slider playerHealthSlider;
        
        [Tooltip("UI distance")]
        [SerializeField] private Vector3 screenOffset = new Vector3(0f, 30f, 0f);
 
        float characterControllerHeight = 0f;
        private PlayerManager target;

        Transform targetTransform;
        Vector3 targetPosition;

        private void Awake()
        {
            transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        }

        private void Update()
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }
        }

        public void SetTarget(PlayerManager _target)
        {
            if (_target == null)
            {
                Debug.LogError("Passed PlayerManager Instance is null", this);
                return;
            }

            target = _target;
            if (playerNameText != null)
            {
                playerNameText.text = target.GetPhotonView().Owner.NickName;
            }
        }
    }
}

