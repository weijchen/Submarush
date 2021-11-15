using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LaserObj : MonoBehaviour
{
    [SerializeField] private GameObject laserBeam;
    [SerializeField] private float waitForLaser = 1.0f;
    [SerializeField] private float lastForLaserMax = 4.0f;
    [SerializeField] private int numberOfLaserMax = 6;
    [SerializeField] private ParticleSystem[] laserSparks;

    private int numberOfLaser = 0;
    
    private void Start()
    {
        laserBeam.SetActive(false);
    }

    private void Update()
    {
        if (numberOfLaser == numberOfLaserMax)
        {
            laserBeam.SetActive(false);
            foreach (ParticleSystem laserSpark in laserSparks)
            {
                laserSpark.Stop();
            }
        }
    }

    private void LaunchLaser()
    {
        laserBeam.SetActive(true);
        foreach (ParticleSystem laserSpark in laserSparks)
        {
            laserSpark.Play();
        }
        StartCoroutine(StartLaunchLaser());
    }

    IEnumerator StartLaunchLaser()
    {
        float lastFor;
        while (numberOfLaser < numberOfLaserMax)
        {
            lastFor = Random.Range(0, lastForLaserMax);
            numberOfLaser += 1; 
            yield return new WaitForSeconds(lastFor);
            laserBeam.SetActive(false);
            yield return new WaitForSeconds(waitForLaser);
            laserBeam.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            LaunchLaser();
        }
    }
}
