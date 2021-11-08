using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObj : MonoBehaviour
{
    [SerializeField] private float smoothness = 0.01f;
    [SerializeField] private float duration = 2.0f;
    [SerializeField] private float offset = 2.0f;
    [SerializeField] private bool moveHorizontal = true;
    [SerializeField] private bool startLeft = true;
    [SerializeField] private bool startUp = true;

    private void Start()
    {
        MoveAround();
    }

    public void MoveAround()
    {
        StartCoroutine(StartMoveAround(smoothness, duration));
    }

    IEnumerator StartMoveAround(float smoothness, float duration)
    {
        float progress = 0f;
        float step = smoothness / duration;
        Vector3 offsetVec = Vector3.zero;
        if (moveHorizontal)
        {
            offsetVec = startLeft ? new Vector3(-offset, 0, 0) : new Vector3(offset, 0, 0);
        }
        else
        {
            offsetVec = startUp ? new Vector3(0, -offset, 0) : new Vector3(0, offset, 0);
        }
        
        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + offsetVec, progress * Time.deltaTime);
            progress += step;
            
            if (progress >= duration)
            {
                progress = 0f;
                offsetVec *= -1;
            }

            yield return new WaitForSeconds(smoothness);
        }
    }
}
