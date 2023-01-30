using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FlowFieldGenerator : MonoBehaviour
{
    public const int MAX_COST = 255;

    [SerializeField] private FlowVector _flowVectorPrefab;
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private float cellSize;
    private List<FlowVector> vectors = new List<FlowVector>();
    private FlowVector _lastCellClicked;

    private Camera _cam;
    
    private void Start()
    {
        _cam = Camera.main;

        GenerateGrid();
    }

    private void Update()
    {
        CheckOnClickCell();
    }

    private void GenerateGrid()
    {
        int index = 0;
        for (float x = 0; x < gridSize.x; x += cellSize)
        {
            for (float y = 0; y < gridSize.y; y += cellSize)
            {
                Vector2 position = new Vector2(x * cellSize + (cellSize / 2), y * cellSize + (cellSize / 2));
                FlowVector newVector = Instantiate(_flowVectorPrefab, this.transform);
                Vector2 size = new Vector2(cellSize, cellSize);
                index++;
                
                newVector.Initialize(position, size, index);
                newVector.Cost = CheckCellTraversability(position, size);
                vectors.Add(newVector);
            }
        }
    }

    private void GenerateIntegrationField()
    {
        ResetIntegrationValues();
    }

    private void ResetIntegrationValues()
    {
        foreach (FlowVector vector in vectors)
        {
            vector.Cost = CheckCellTraversability(vector.Position, vector.Size);
        }
    }

    private int CheckCellTraversability(Vector2 position, Vector2 size)
    {
        if (Physics2D.OverlapBox(position, size, 0))
        {
            return MAX_COST;
        }

        return 0;
    }

    private void GenerateFlowField()
    {
        
    }

    private void CheckOnClickCell()
    {
        RaycastHit2D hit = Physics2D.Raycast(_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (Input.GetMouseButtonDown(0))
        {
            if (hit.collider == null)
                return;

            if (hit.transform.TryGetComponent(out FlowVector vector))
            {
                if (_lastCellClicked != null) _lastCellClicked.OnDeselectCell();
                
                _lastCellClicked = vector;
                vector.OnSelectCell();
                
                GenerateIntegrationField();
                GenerateFlowField();
            }
        }
    }
}