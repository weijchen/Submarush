using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObj : MonoBehaviour
{
    [SerializeField] private float anglePerSecond = 0.2f;
    [SerializeField] private Transform rotateCenter;
    [SerializeField] private bool clockwise = true;
    [SerializeField] private bool rotateWithRest = false;
    [SerializeField] private float restTime = 2.0f;

    private float timer = 0f;
    private bool stopRotate = false;
    private float totalRotateAngle;

    private void Update()
    {
        RotateSelf();
    }

    private void RotateSelf()
    {
        if (!stopRotate)
        {
            if (clockwise)
            {
                transform.RotateAround(rotateCenter.position, Vector3.forward, -anglePerSecond);
            }
            else
            {
                transform.RotateAround(rotateCenter.position, Vector3.forward, anglePerSecond);
            }
        }

        totalRotateAngle += Mathf.Abs(anglePerSecond);
        if (rotateWithRest)
        {
            if (totalRotateAngle >= 360.0f)
            {
                stopRotate = true;
                if (stopRotate)
                {
                    timer += Time.deltaTime;
                    if (timer >= restTime)
                    {
                        totalRotateAngle = 0f;
                        clockwise = !clockwise;
                        timer = 0f;
                        stopRotate = false;
                    }
                }
            }
        }
    }
}
