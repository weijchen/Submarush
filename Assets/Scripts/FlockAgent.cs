using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FlockAgent : MonoBehaviour
{
    private Flock agentFlock;

    public Flock AgentFlock
    {
        get { return agentFlock; }
    }
    
    private Collider agentCollider;
    public Collider AgentCollider
    {
        get { return agentCollider; }
    }
    
    private void Start()
    {
        agentCollider = GetComponent<Collider>();
    }

    public void Initialize(Flock flock)
    {
        agentFlock = flock;
    }

    public void Move(Vector3 velocity)
    {
        transform.up = velocity;
        transform.position += velocity * Time.deltaTime;
    }
}
