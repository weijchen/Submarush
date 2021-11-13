using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorObj : MonoBehaviour
{
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private Transform leftEndTransform;
    [SerializeField] private GameObject rightDoor;
    [SerializeField] private Transform rightEndTransform;
    [SerializeField] private float smoothness = 0.01f;
    [SerializeField] private float duration = 2f;

    public void OpenDoor()
    {
        StartCoroutine(StartOpenDoor(smoothness, duration));
    }

    IEnumerator StartOpenDoor(float smoothness, float duration)
    {
        float step = smoothness / duration;
        float progress = 0f;
        
        while (progress <= 1)
        {
            leftDoor.transform.position = Vector3.Lerp(leftDoor.transform.position, leftEndTransform.position, progress);
            rightDoor.transform.position = Vector3.Lerp(rightDoor.transform.position, rightEndTransform.position, progress);
            progress += step;
            yield return new WaitForSeconds(smoothness);
        }
    }
}
