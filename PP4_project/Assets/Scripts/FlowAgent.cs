using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowAgent : MonoBehaviour
{
    public Vector3 CurrentDirection;
    public Rigidbody RB;
    public Vector3 TargetPosition;
    
    [Header("Speed")]
    public float MoveSpeed;
    public float MaxSpeed = 2;
    public float StoppingDistance = 0.5f;
   
    private void FixedUpdate()
    {
        if (CurrentDirection.magnitude == 0)
        {
            RB.velocity *= 0.5f;
            return;
        }
        
        if (RB.velocity.magnitude >= MaxSpeed)
        {
            RB.velocity *= 0.99f;
        }
        
        if (Vector3.Distance(transform.position, TargetPosition) <= StoppingDistance)
        {
            RB.velocity = Vector3.zero;
        }
        
        RB.velocity += CurrentDirection * (MoveSpeed * Time.deltaTime);
    }
}