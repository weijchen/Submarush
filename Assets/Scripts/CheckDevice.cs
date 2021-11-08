using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace Team73.Round5.Racing
{    
    public class CheckDevice : MonoBehaviour
    {
        public SteamVR_TrackedObject Device1;
        public SteamVR_TrackedObject Device2;

        void Start()
        {
            Device1 = FindObjectsOfType<SteamVR_TrackedObject>()[0];
            Device2 = FindObjectsOfType<SteamVR_TrackedObject>()[1];
            
            uint index1 = 0;
            uint index2 = 0;
            var error = ETrackedPropertyError.TrackedProp_Success;
            for (uint i = 0; i < 16; i++)
            {
                var result = new System.Text.StringBuilder((int)64);
                
                OpenVR.System.GetStringTrackedDeviceProperty(i, ETrackedDeviceProperty.Prop_RenderModelName_String, result, 64, ref error);
                if (result.ToString().Contains("tracker"))
                {
                    if (index1 == 0 && i != 0) 
                    {
                        index1 = i;
                    }   
                    
                    if (index1 != 0 && index1 != i)
                    {
                        index2 = i;
                    }
                }
            }
            Device1.GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)index1;
            Device2.GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)index2;
        }
    }
}
