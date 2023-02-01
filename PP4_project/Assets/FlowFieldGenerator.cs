using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FlowFieldGenerator : MonoBehaviour
{
    public const int MAX_COST = 255;

    [SerializeField] private FlowVector _flowVectorPrefab;
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private float cellSize;
    private FlowVector _currentCell;

    private List<FlowVector> _openCells = new List<FlowVector>();
    private List<FlowVector> _visitedCells = new List<FlowVector>();

    private Dictionary<Vector2, FlowVector> _cellsInGrid = new Dictionary<Vector2, FlowVector>();

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
        Vector2 index = Vector2.zero;
        for (float x = 0; x < gridSize.x; x += cellSize)
        {
            for (float y = 0; y < gridSize.y; y += cellSize)
            {
                Vector2 position = new Vector2(x * cellSize + (cellSize / 2), y * cellSize + (cellSize / 2));
                FlowVector newVector = Instantiate(_flowVectorPrefab, this.transform);
                index = new Vector2(x, y);
                newVector.transform.name = $"Cell {index}";
                Vector2 size = new Vector2(cellSize, cellSize);
                

                newVector.Initialize(position, size, index);
                newVector.Cost = CheckCellTraversability(position, size);
                _cellsInGrid.Add(index, newVector);
            }
        }

        foreach (var cell in _cellsInGrid)
        {
            Vector2 currentIndex = cell.Value.Index;
            //North
            Vector2 northIndex = new Vector2(currentIndex.x, currentIndex.y + 1);
            if (_cellsInGrid.TryGetValue(northIndex, out FlowVector northCell))
                cell.Value.AddNeighbour(northCell);
            //East
            Vector2 eastIndex = new Vector2(currentIndex.x + 1, currentIndex.y);
            if (_cellsInGrid.TryGetValue(eastIndex, out FlowVector eastCell))
                cell.Value.AddNeighbour(eastCell);

            //South
            Vector2 southIndex = new Vector2(currentIndex.x, currentIndex.y - 1);
            if (_cellsInGrid.TryGetValue(southIndex, out FlowVector southCell))
                cell.Value.AddNeighbour(southCell);

            //West
            Vector2 westIndex = new Vector2(currentIndex.x - 1, currentIndex.y);
            if (_cellsInGrid.TryGetValue(westIndex, out FlowVector westCell))
                cell.Value.AddNeighbour(westCell);
        }
    }

    private void GenerateIntegrationField(FlowVector pCurrentCell, int pLastTentativeDist)
    {
        _openCells.Add(pCurrentCell);
        while (_openCells.Count != 0)
        {
            FlowVector selectedCell = _openCells.First();

            foreach (var neighbour in selectedCell.NeighbourCells)
            {
                //Cell is unpassable (wall)
                if (selectedCell.Cost == MAX_COST) continue;

                if (_visitedCells.Contains(selectedCell))
                {
                    //if(dist.n > dist.c + a)
                    //dist.n = dist.c + a
                    continue;
                }

                //dist.n = dist.c + a
                //openCells.add(neighbour)
            }
        }
    }

    private int CalculateCost(Vector2 pCurrentCell, Vector2 pTargetCell)
    {
        return (int)Vector2.Distance(pCurrentCell, pTargetCell);
    }

    private void ResetIntegrationValues()
    {
        foreach (KeyValuePair<Vector2, FlowVector> vector in _cellsInGrid)
        {
            vector.Value.Cost = CheckCellTraversability(vector.Value.Position, vector.Value.Size);
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
                if (_currentCell != null) _currentCell.OnDeselectCell();
                
                _currentCell = vector;
                vector.OnSelectCell();
                
                GenerateIntegrationField(_currentCell, 0);
                GenerateFlowField();
            }
        }
    }
}