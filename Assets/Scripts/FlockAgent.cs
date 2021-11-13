using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FlockAgent : MonoBehaviour
{
    private Collider agentCollider;
    public Collider AgentCollider
    {
        get { return agentCollider; }
    }
    

    private void Start()
    {
        agentCollider = GetComponent<Collider>();
    }

    public void Move(Vector3 velocity)
    {
        transform.up = velocity;
        transform.position += velocity * Time.deltaTime;
    }
}
