using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [SerializeField] private int _maxAgentCount = 10;
    [SerializeField] private GameObject _agentPrefab;
    private List<FlowAgent> _spawnedAgents = new List<FlowAgent>();
    [SerializeField] private FlowFieldGenerator _flowFieldGenerator;

    private void Start()
    {
        for (int i = 0; i < _maxAgentCount; i++)
        {
            GameObject agentGO = Instantiate(_agentPrefab, _flowFieldGenerator.GetRandomPassablePosition(), Quaternion.identity, transform);
            _spawnedAgents.Add(agentGO.GetComponent<FlowAgent>());
        }
    }

    private void Update()
    {
        if (_flowFieldGenerator.CurrentCell == null) return;
        
        foreach (var agent in _spawnedAgents)
        {
            Vector2 currentAgentDirection = _flowFieldGenerator.LookUpDirectionFromCell(agent.transform.position);

            agent.TargetPosition = _flowFieldGenerator.CurrentCell.Position;
            agent.CurrentDirection = currentAgentDirection;
        }
    }
}
