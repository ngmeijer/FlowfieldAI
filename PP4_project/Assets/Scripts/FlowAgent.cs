using System;
using System.Collections.Generic;
using Unity.Mathematics;
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

    [SerializeField] private Vector3 _centerOfGroup;
    [SerializeField] private Vector3 _finalDirection;
    [SerializeField] private Vector3 _flockingVec;
    public bool CurrentlySelected;

    
    private void Start()
    {
        OriginalColor = _renderer.color;
    }

    public void SetRandomVelocity(float xVel, float yVel)
    {
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
        _centerOfGroup = transform.position;

        Vector2 alignmentVec = CalculateAlignment();
        Vector2 cohesionVec = CalculateCohesion();
        Vector2 avoidanceVec = CalculateSeparation();

        _flockingVec = (avoidanceVec * Properties.AvoidanceForce) 
                       + (cohesionVec * Properties.CohesionForce) 
                       + (alignmentVec * Properties.AlignmentForce);
        _finalDirection = transform.up + _flockingVec;
        _finalDirection.Normalize();
        
        RB.velocity = _finalDirection * (Properties.MoveSpeed * Time.fixedDeltaTime);
        Debug.DrawRay(transform.position, _finalDirection, Color.magenta);
    }

    private void LookAtTarget()
    {
        float angle = Mathf.Atan2(CellDirection.y, CellDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Quaternion rotateTowards = Quaternion.RotateTowards(transform.rotation, targetRotation,
            Properties.RotateSpeed * Time.deltaTime);
        transform.rotation = rotateTowards;
    }

    private void GetClosestAgents()
    {
        _neighbourAgents = AIManager.Instance.GetNeighbours(this);
        _neighbourCount = _neighbourAgents.Count;

        foreach (var neighbour in _neighbourAgents)
        {
            Debug.DrawLine(transform.position, neighbour.transform.position, Color.green);
        }

        float dynamicChannelStrength = ((float)_neighbourCount / AIManager.Instance.TotalAgentCount)* ColorMultiplicationFactor;
        float constantChannelStrength = 0.3f;
        Color agentColor = new Color(constantChannelStrength, dynamicChannelStrength, constantChannelStrength);
        if(!CurrentlySelected)
            SetColor(agentColor);
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

        alignmentVec /= _neighbourCount + 1;

        return alignmentVec;
    }

    private Vector2 CalculateCohesion()
    {
        Vector3 cohesionVec = transform.position;

        if (_neighbourCount == 0)
            return cohesionVec;
        
        foreach (var neighbour in _neighbourAgents)
        {
            cohesionVec += neighbour.transform.position; 
        }
        
        cohesionVec /= (_neighbourCount + 1);
        _centerOfGroup = cohesionVec;

        Debug.DrawLine(transform.position, _centerOfGroup, Color.white);
        cohesionVec -= transform.position;

        return cohesionVec;
    }

    private Vector2 CalculateSeparation()
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
        Debug.DrawRay(transform.position + avoidanceVec.normalized * 0.1f, avoidanceVec.normalized * 0.3f, Color.green);

        avoidanceVec /= _neighbourCount + 1;

        return avoidanceVec;
    }

    private void SetColor(Color color)
    {
        _renderer.color = color;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(_centerOfGroup, 0.05f);
        
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
        }
        else
        {
            SetColor(OriginalColor);
            CurrentlySelected = false;
        }
    }
}