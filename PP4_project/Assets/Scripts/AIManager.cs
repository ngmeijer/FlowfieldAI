using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[SelectionBase]
public class AIManager : SingletonMonobehaviour<AIManager>
{
    [SerializeField] private int _maxAgentCount = 10;
    [SerializeField] private GameObject _agentPrefab;
    private List<FlockingAgent> _spawnedAgents = new List<FlockingAgent>();
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
            // float randomNumber = Random.Range(0f, 1f);
            // if (randomNumber > _randomChance) 
            //     continue;
            
            // GameObject agentGO = Instantiate(_agentPrefab, _flowFieldGenerator.GetRandomPassablePosition(), Quaternion.identity, transform);

            Vector3 randomPos = new Vector3(Random.Range(-75f, 75f), Random.Range(-75f, 75f), 0f);
            GameObject agentGO = Instantiate(_agentPrefab, randomPos, Quaternion.identity, transform);
            _spawnedAgents.Add(agentGO.GetComponent<FlockingAgent>());
        }
        
        TotalAgentCount = _spawnedAgents.Count;
    }

    public void ApplyVelocityToAgents()
    {
        foreach (var agent in _spawnedAgents)
        {
            //agent.SetRandomVelocity();
        }
    }

    public void ApplyRotationToAgents()
    {
        foreach (var agent in _spawnedAgents)
        {
            float zRot = Random.Range(0f, 360f);
            //agent.SetRandomRotation(zRot);
        }
    }

    private void Update()
    {
        // if (_flowFieldGenerator.CurrentCell == null) return;
        //
        // foreach (var agent in _spawnedAgents)
        // {
        //     Vector2 currentAgentDirection = _flowFieldGenerator.LookUpDirectionFromCell(agent.transform.position);
        //
        //     agent.CellDirection = currentAgentDirection;
        // }
        
        HighlightAgent();
    }

    private void HighlightAgent()
    {
        // if (_selectedAgentIndex < -1)
        //     _selectedAgentIndex = -1;
        // if (_selectedAgentIndex >= _spawnedAgents.Count)
        //     _selectedAgentIndex = _spawnedAgents.Count - 1;
        //
        // if (_currentAgent != null)
        //     _currentAgent.SelectAgent(false);
        //
        // if (_selectedAgentIndex == -1)
        //     return;
        // _currentAgent = _spawnedAgents[_selectedAgentIndex];
        // _currentAgent.SelectAgent(true);
    }

    // public List<FlowAgent> GetNeighbours(FlowAgent givenAgent)
    // {
    //     List<FlowAgent> neighbours = new List<FlowAgent>();
    //     foreach (FlowAgent potentialNeighbour in _spawnedAgents)
    //     {
    //         if (potentialNeighbour == givenAgent) 
    //             continue;
    //         if (Vector3.Distance(givenAgent.transform.position, potentialNeighbour.transform.position) >
    //             givenAgent.Properties.AlignmentRadius)
    //             continue;
    //         
    //         neighbours.Add(potentialNeighbour);
    //     }
    //     
    //     return neighbours;
    // }
}
