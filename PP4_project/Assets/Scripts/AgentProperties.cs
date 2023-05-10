using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "AgentProperties", menuName = "ScriptableObjects/AgentProperties", order = 1)]
public class AgentProperties : ScriptableObject
{
    [Range(0f, 3f)]public float MoveSpeed;
    [Range(0f, 3f)]public float MaxSpeed = 2;
    public float StoppingDistance = 0.5f;
    public float RotateSpeed = 1f;
    
    [Range(1f, 5f)] public float CohesionForce = 2f;
    [Range(0f, 10f)] public float AlignmentRadius = 1f;
    [Range(1f, 5f)] public float AlignmentForce = 2f;
    public Color AlignmentDebugColor;
    
    [Range(0f, 10f)] public float AvoidanceRadius = 1f;
    [Range(1f, 5f)] public float AvoidanceForce = 2f;
    public Color AvoidanceDebugColor;

    public bool ShowRadius;
}