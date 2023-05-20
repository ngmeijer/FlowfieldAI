using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlowAgent : MonoBehaviour
{
    public Vector3 CellDirection;
    public Rigidbody RB;
    [SerializeField] private SpriteRenderer _renderer;
    public AgentProperties Properties;
    [SerializeField] private List<FlowAgent> _neighbourAgents;
    private int _neighbourCount;
    public Color OriginalColor;
    [SerializeField] private float ColorMultiplicationFactor = 3f;

    [SerializeField] private Vector3 _finalDirection;
    [SerializeField] private Vector3 _flockingVec;
    private Vector3 _alignmentVec;
    private Vector3 _cohesionVec;
    private Vector3 _avoidanceVec;
    public bool CurrentlySelected;

    
    private void Start()
    {
        OriginalColor = _renderer.color;
    }

    public void SetRandomVelocity()
    {
        float xVel = NumberRandomizer.GetRandomFloat(-1f, 100f);
        float yVel = NumberRandomizer.GetRandomFloat(-1f, 1f);
        RB.velocity = new Vector3(xVel, yVel) * Properties.MoveSpeed;
    }

    public void SetRandomRotation(float zRot)
    {
        transform.eulerAngles = new Vector3(0,0,zRot);
    }

    private void Update()
    {
        //if (CurrentDirection == Vector3.zero) return;

        //LookAtTarget();
        
        GetClosestAgents();
    }

    private void FixedUpdate()
    {
        _alignmentVec = CalculateAlignment();
        _cohesionVec = CalculateCohesion();
        _avoidanceVec = CalculateSeparation();

        _flockingVec = (_avoidanceVec * Properties.AvoidanceForce) + 
                       (_cohesionVec * Properties.CohesionForce) + 
                       (_alignmentVec * Properties.AlignmentForce);
        _flockingVec.Normalize();
        _finalDirection = (_flockingVec * Properties.FlockingWeight) + (CellDirection * Properties.DirectionWeight);
        _finalDirection.Normalize();
        
        Debug.DrawRay(transform.position, _finalDirection, Color.magenta);
        Debug.DrawRay(transform.position, transform.up, Color.yellow);

        float moveSpeed = Properties.MoveSpeed * Time.fixedDeltaTime;
        Vector3 movement = _finalDirection * moveSpeed;
        RB.MovePosition(transform.position + movement);
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
        // _neighbourAgents = AIManager.Instance.GetNeighbours(this);
        // _neighbourCount = _neighbourAgents.Count;

        foreach (var neighbour in _neighbourAgents)
        {
            Debug.DrawLine(transform.position, neighbour.transform.position, Color.grey);
        }

        float dynamicChannelStrength = ((float)_neighbourCount / AIManager.Instance.TotalAgentCount)* ColorMultiplicationFactor;
        float constantChannelStrength = 0.3f;
        Color agentColor = new Color(constantChannelStrength, dynamicChannelStrength, constantChannelStrength);
        if(!CurrentlySelected)
            SetColor(agentColor);
    }

    private Vector3 CalculateAlignment()
    {
        Vector3 alignmentVec = RB.velocity;
        if (_neighbourCount == 0)
        {
            return alignmentVec;
        }

        foreach (var neighbour in _neighbourAgents)
        {
            alignmentVec += neighbour.RB.velocity; 
        }

        alignmentVec /= _neighbourCount;
        Debug.DrawRay(transform.position, alignmentVec, Color.blue);

        return alignmentVec;
    }

    private Vector3 CalculateCohesion()
    {
        Vector3 cohesionVec = transform.position;

        if (_neighbourCount == 0)
            return cohesionVec;
        
        foreach (var neighbour in _neighbourAgents)
        {
            cohesionVec += neighbour.transform.position; 
        }
        
        cohesionVec /= _neighbourCount;
        cohesionVec -= transform.position;
        Debug.DrawLine(transform.position, cohesionVec, Color.green);

        return cohesionVec;
    }

    private Vector3 CalculateSeparation()
    {
        Vector3 avoidanceVec = Vector3.zero;

        if (_neighbourCount == 0)
            return avoidanceVec;

        avoidanceVec += transform.position;

        foreach (var neighbour in _neighbourAgents)
        {
            float distance = Vector3.Distance(transform.position, neighbour.transform.position);
            if (distance >= Properties.AvoidanceRadius)
                continue;
            
            avoidanceVec -= neighbour.transform.position;
        }
        
        //Shows direction to move in
        Debug.DrawRay(transform.position, avoidanceVec.normalized * 0.3f, Color.red);

        avoidanceVec /= _neighbourCount;

        return avoidanceVec;
    }

    private void SetColor(Color color)
    {
        _renderer.color = color;
    }

    private void OnDrawGizmos()
    {
        Handles.Label(transform.position + transform.up, "Up vector");
        Handles.Label(transform.position + _finalDirection, "Final direction");
        Handles.Label(transform.position + _avoidanceVec, "Avoidance");
        Handles.Label(transform.position + _alignmentVec, "Alignment");
        
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(_cohesionVec, 0.05f);
        
        if (!Properties.ShowRadius) return;
        
        Gizmos.color = Properties.AlignmentDebugColor;
        Gizmos.DrawSphere(transform.position, Properties.AlignmentRadius);

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