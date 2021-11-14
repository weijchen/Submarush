using System;
using System.Collections;
using System.Collections.Generic;
using Team73.Round5.Racing;
using UnityEngine;

namespace Team73.Round5.Racing
{
    public class DoorBtn : MonoBehaviour
    {
        [SerializeField] private DoorObj _doorObj;
        [SerializeField] private GameObject doorSymbol;

        private void OnCollisionEnter(Collision other)
        {
            if (other.transform.CompareTag("Player"))
            {
                _doorObj.OpenDoor();
                SoundManager.Instance.PlaySFXByIndex(SFXList.Door);
                gameObject.SetActive(false);
                doorSymbol.SetActive(false);
            }
        }
    }
}
