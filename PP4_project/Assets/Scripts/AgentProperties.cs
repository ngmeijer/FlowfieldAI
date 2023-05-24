using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "AgentProperties", menuName = "ScriptableObjects/AgentProperties", order = 1)]
public class AgentProperties : ScriptableObject
{
    [Range(0f, 3f)]public float FlockWeight;
    [Range(0f, 3f)]public float MoveSpeed;
    [Range(0f, 3f)]public float MaxSpeed = 2;
    public float StoppingDistance = 0.5f;
    public float RotateSpeed = 1f;
    [Range(0f, 10f)] public float MaxGroupingDistance = 1f;
    public float FlockingWeight;
    public float DirectionWeight;
    
    [Header("Cohesion")]
    [Range(1f, 50f)] public float CohesionForce = 2f;
    
    [Header("Alignment")]
    [Range(1f, 50f)] public float AlignmentForce = 2f;
    public Color AlignmentDebugColor;
    
    [Header("Avoidance")]
    [Range(0f, 10f)] public float AvoidanceRadius = 1f;
    [Range(1f, 50f)] public float AvoidanceForce = 2f;
    public Color AvoidanceDebugColor;

    public bool ShowRadius;
}