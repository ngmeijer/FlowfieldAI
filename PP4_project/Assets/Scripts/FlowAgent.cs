using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowAgent : MonoBehaviour
{
    public Vector3 CurrentDirection = Vector3.right;
    public float MoveSpeed;
    
    private void Update()
    {
        transform.position += (CurrentDirection / MoveSpeed) * Time.deltaTime;
    }
}
