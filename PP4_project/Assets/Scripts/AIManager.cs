using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIManager : SingletonMonobehaviour<AIManager>
{
    [SerializeField] private int _maxAgentCount = 10;
    [SerializeField] private GameObject _agentPrefab;
    private List<FlowAgent> _spawnedAgents = new List<FlowAgent>();
    public int TotalAgentCount { get; private set; }
    [SerializeField] private FlowFieldGenerator _flowFieldGenerator;
    [SerializeField] private float _randomChance = 1f;
    [SerializeField] private int _selectedAgentIndex;
    [SerializeField] private FlowAgent _currentAgent;

    [SerializeField] private List<Color> _availableColours;

    private void Start()
    {
        for (int i = 0; i < _maxAgentCount; i++)
        {
            float randomNumber = Random.Range(0f, 1f);
            if (randomNumber > _randomChance) 
                continue;
            
            GameObject agentGO = Instantiate(_agentPrefab, _flowFieldGenerator.GetRandomPassablePosition(), Quaternion.identity, transform);
            _spawnedAgents.Add(agentGO.GetComponent<FlowAgent>());
        }

        

        TotalAgentCount = _spawnedAgents.Count;
    }

    public void ApplyVelocityToAgents()
    {
        foreach (var agent in _spawnedAgents)
        {
            float xVel = Random.Range(-1f, 1f);
            float yVel = Random.Range(-1f, 1f);
            agent.SetRandomVelocity(xVel, yVel);
        }
    }

    public void ApplyRotationToAgents()
    {
        foreach (var agent in _spawnedAgents)
        {
            float zRot = Random.Range(0f, 360f);
            agent.SetRandomRotation(zRot);
        }
    }

    private void Update()
    {
        //if (_flowFieldGenerator.CurrentCell == null) return;
        
        foreach (var agent in _spawnedAgents)
        {
            Vector2 currentAgentDirection = _flowFieldGenerator.LookUpDirectionFromCell(agent.transform.position);

            agent.CellDirection = currentAgentDirection;
        }
    }

    private void FixedUpdate()
    {
        HighlightAgent();
    }

    private void HighlightAgent()
    {
        if (_selectedAgentIndex < -1)
            _selectedAgentIndex = -1;
        if (_selectedAgentIndex >= _spawnedAgents.Count)
            _selectedAgentIndex = _spawnedAgents.Count - 1;

        if (_currentAgent != null)
            _currentAgent.SelectAgent(false);

        if (_selectedAgentIndex == -1)
            return;
        _currentAgent = _spawnedAgents[_selectedAgentIndex];
        _currentAgent.SelectAgent(true);
    }

    public List<FlowAgent> GetNeighbours(FlowAgent givenAgent)
    {
        List<FlowAgent> neighbours = new List<FlowAgent>();
        foreach (FlowAgent potentialNeighbour in _spawnedAgents)
        {
            if (potentialNeighbour == givenAgent) 
                continue;
            if (Vector3.Distance(givenAgent.transform.position, potentialNeighbour.transform.position) >
                givenAgent.Properties.AlignmentRadius)
                continue;
            
            neighbours.Add(potentialNeighbour);
        }
        
        return neighbours;
    }
}
