using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RotateDir: int
{
    Forward = 1,
    Right = 2,
    Up = 3
}

public class RotateObj : MonoBehaviour
{
    [SerializeField] private float anglePerSecond = 0.2f;
    [SerializeField] private Transform rotateCenter;
    [SerializeField] private bool clockwise = true;
    [SerializeField] private bool rotateWithRest = false;
    [SerializeField] private float restTime = 2.0f;
    [SerializeField] private RotateDir _rotateDir = RotateDir.Forward;

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
                if (_rotateDir == RotateDir.Forward)
                {
                    transform.RotateAround(rotateCenter.position, Vector3.forward, -anglePerSecond);
                } 
                else if (_rotateDir == RotateDir.Right)
                {
                    transform.RotateAround(rotateCenter.position, Vector3.right, -anglePerSecond);
                }
                else
                {
                    transform.RotateAround(rotateCenter.position, Vector3.up, -anglePerSecond);
                }
            }
            else
            {
                if (_rotateDir == RotateDir.Forward)
                {
                    transform.RotateAround(rotateCenter.position, Vector3.forward, anglePerSecond);
                } 
                else if (_rotateDir == RotateDir.Right)
                {
                    transform.RotateAround(rotateCenter.position, Vector3.right, anglePerSecond);
                }
                else
                {
                    transform.RotateAround(rotateCenter.position, Vector3.up, anglePerSecond);
                }
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
