using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlowAgent : MonoBehaviour
{
    private const float COLOR_MULTIPLIER = 10f;
    public Vector3 CellDirection;
    public Rigidbody RB;
    [SerializeField] private SpriteRenderer _renderer;
    public AgentProperties Properties;
    [SerializeField] private List<FlowAgent> _neighbourAgents;
    private int _neighbourCount;
    public Color OriginalColor;

    [SerializeField] private Vector3 _finalDirection;
    [SerializeField] private Vector3 _flockingVec;
    private Vector3 _alignmentVec;
    private Vector3 _cohesionVec;
    private Vector3 _avoidanceVec;
    public bool CurrentlySelected;
    private System.Random randomGenerator;
    public int ID;

    [HideInInspector] public bool _showAlignment;
    [HideInInspector] public bool _showCohesion;
    [HideInInspector] public bool _showAvoidance;
    [HideInInspector] public bool _showUpVector;
    [HideInInspector] public bool _showFinalDirection;
    
    private void Start()
    {
        OriginalColor = _renderer.color;
        randomGenerator = new System.Random(ID);
        SetRandomVelocity();
        SetRandomRotation();
    }

    private void SetRandomVelocity()
    {
        float xVel = randomGenerator.Next(-100, 100) / 100f;
        float yVel = randomGenerator.Next(-100, 100) / 100f;
        RB.velocity = new Vector3(xVel, yVel) * Properties.FlockWeight;
    }

    private void SetRandomRotation()
    {
        float zRot = randomGenerator.Next(0, 360);
        transform.eulerAngles = new Vector3(0,0,zRot);
    }

    private void Update()
    {
        GetClosestAgents();
    }

    private void FixedUpdate()
    {
        _alignmentVec = CalculateAlignment() * Properties.AlignmentForce;
        _cohesionVec = CalculateCohesion() * Properties.CohesionForce;
        _avoidanceVec = CalculateAvoidance() * Properties.AvoidanceForce;

        _flockingVec = _avoidanceVec + _cohesionVec + _alignmentVec;
        
        //Debugging
        if(_showFinalDirection)
            Debug.DrawRay(transform.position, _finalDirection, Color.magenta);
        if(_showUpVector)
            Debug.DrawRay(transform.position, transform.up, Color.yellow);

        float flockWeight = Properties.FlockWeight * Time.fixedDeltaTime;
        Vector3 movement = _flockingVec * flockWeight + (transform.up * Properties.MoveSpeed);
        RB.velocity = movement;
    }

    private void LookAtTarget()
    {
        float angle = Mathf.Atan2(CellDirection.y, CellDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, _finalDirection);
        Quaternion rotateTowards = Quaternion.RotateTowards(transform.rotation, targetRotation,
            Properties.RotateSpeed * Time.deltaTime);
        transform.rotation = rotateTowards;
    }

    private void GetClosestAgents()
    {
        _neighbourAgents = AIManager.Instance.GetNeighbours(this);
        _neighbourCount = _neighbourAgents.Count;

        float lerpProgress = ((float)_neighbourCount / AIManager.Instance.TotalAgentCount) * COLOR_MULTIPLIER;
        Color noNeighbourColor = Color.red;
        Color hasNeighbourColor = Color.green;
        Color lerpedColor = Color.Lerp(noNeighbourColor, hasNeighbourColor, lerpProgress);
        if(!CurrentlySelected)
            SetColor(lerpedColor);
    }

    private Vector3 CalculateAlignment()
    {
        Vector3 alignmentVec = transform.up;
        if (_neighbourCount == 0)
        {
            return alignmentVec;
        }

        foreach (var neighbour in _neighbourAgents)
        {
            alignmentVec += neighbour.transform.up; 
        }

        alignmentVec /= _neighbourCount;
        
        if(_showAlignment)
            Debug.DrawRay(transform.position, alignmentVec, Color.blue);
        
        return alignmentVec;
    }

    private Vector3 CalculateCohesion()
    {
        Vector3 cohesionVec = Vector3.zero;

        if (_neighbourCount == 0)
            return cohesionVec;
        
        foreach (var neighbour in _neighbourAgents)
        {
            cohesionVec += neighbour.transform.position; 
        }
        
        cohesionVec /= _neighbourCount;
        cohesionVec -= transform.position;
        
        if(_showCohesion)
            Debug.DrawLine(transform.position, cohesionVec, Color.green);

        return cohesionVec;
    }

    private Vector3 CalculateAvoidance()
    {
        Vector3 avoidanceVec = Vector3.zero;

        if (_neighbourCount == 0)
            return avoidanceVec;
        
        foreach (var neighbour in _neighbourAgents)
        {
            float distance = Vector3.Distance(transform.position, neighbour.transform.position);
            if (distance >= Properties.AvoidanceRadius)
                continue;

            avoidanceVec -= (neighbour.transform.position - transform.position);
        }
        
        if(_showAvoidance)
            Debug.DrawRay(transform.position, avoidanceVec.normalized, Color.red);

        avoidanceVec /= _neighbourCount;

        return avoidanceVec;
    }

    private void SetColor(Color newColor)
    {
        _renderer.color = newColor;
    }

    private void OnDrawGizmos()
    {
        if(_showUpVector)
            Handles.Label(transform.position + transform.up, "Up vector");
        if(_showFinalDirection)
            Handles.Label(transform.position + _finalDirection, "Final direction");
        if(_showAvoidance)
            Handles.Label(transform.position + _avoidanceVec, "Avoidance");
        if(_showAlignment)
            Handles.Label(transform.position + _alignmentVec, "Alignment");

        if (_showCohesion)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(_cohesionVec, 0.05f);
        }

        if (!Properties.ShowRadius) return;
        
        Gizmos.color = Properties.AlignmentDebugColor;
        Gizmos.DrawSphere(transform.position, Properties.MaxGroupingDistance);

        Gizmos.color = Properties.AvoidanceDebugColor;
        Gizmos.DrawSphere(transform.position, Properties.AvoidanceRadius);
    }

    public void SelectAgent(bool select)
    {
        if (select)
        {
            SetColor(Color.yellow);
            CurrentlySelected = true;
            Debug.Log($"Flocking vec: {_flockingVec}");
        }
        else
        {
            SetColor(OriginalColor);
            CurrentlySelected = false;
        }
    }
}

public static class NumberRandomizer
{
    public static float GetRandomFloat(float min, float max)
    {
        return Random.Range(min, max);
    }
}