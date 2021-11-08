using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShakeObj : MonoBehaviour
{
    [SerializeField] float amount = 1.0f;
    [SerializeField] private float smoothness = 0.01f;

    private Vector3 newPosition;

    private void Start()
    {
        Shake(smoothness);
    }
    
    private void Shake(float smoothness)
    {
        StartCoroutine(StartShake(smoothness));
    }

    IEnumerator StartShake(float smoothness)
    {
        float randomXOffset = Random.Range(0, amount);
        float randomYOffset = Random.Range(0, amount);
        
        while (true)
        {
            newPosition = new Vector3(transform.position.x + randomXOffset, transform.position.y + randomYOffset,
                transform.position.z);
            transform.position = newPosition;
            yield return new WaitForSeconds(smoothness);
            newPosition = new Vector3(transform.position.x - randomXOffset, transform.position.y - randomYOffset,
                transform.position.z);
            transform.position = newPosition;
        }
    }
}
