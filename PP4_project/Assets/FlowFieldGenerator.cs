using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FlowFieldGenerator : MonoBehaviour
{
    public const int MAX_COST = 255;
    private Camera _cam;

    [SerializeField] private FlowVector _flowVectorPrefab;
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private float cellSize;
    
    private FlowVector _currentCell;
    private Queue<FlowVector> _openCells = new Queue<FlowVector>();
    private List<FlowVector> _visitedCells = new List<FlowVector>();
    private Dictionary<Vector2, FlowVector> _cellsInGrid = new Dictionary<Vector2, FlowVector>();
    
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
        Vector2 size = new Vector2(cellSize, cellSize);
        Vector2 innerOffset = new Vector2(0.05f * size.x, 0.05f * size.y);

        for (float x = 0; x < gridSize.x; x += cellSize)
        {
            for (float y = 0; y < gridSize.y; y += cellSize)
            {
                Vector2 position = new Vector2(x * cellSize + (cellSize / 2), y * cellSize + (cellSize / 2));
                FlowVector newVector = Instantiate(_flowVectorPrefab, this.transform);
                newVector.transform.name = $"Cell {position}";
                
                int cost = CheckCellTraversability(position, size - innerOffset);
                
                newVector.Initialize(position, size, cost);
                _cellsInGrid.Add(position, newVector);
            }
        }

        foreach (var cell in _cellsInGrid)
        {
            Vector2 currentCellPosition = cell.Value.Position;
            //North
            Vector2 northPos = new Vector2(currentCellPosition.x, currentCellPosition.y + 1);
            if (_cellsInGrid.TryGetValue(northPos, out FlowVector northCell))
                cell.Value.AddNeighbour(northCell);
            //East
            Vector2 eastPos = new Vector2(currentCellPosition.x + 1, currentCellPosition.y);
            if (_cellsInGrid.TryGetValue(eastPos, out FlowVector eastCell))
                cell.Value.AddNeighbour(eastCell);

            //South
            Vector2 southPos = new Vector2(currentCellPosition.x, currentCellPosition.y - 1);
            if (_cellsInGrid.TryGetValue(southPos, out FlowVector southCell))
                cell.Value.AddNeighbour(southCell);

            //West
            Vector2 westPos = new Vector2(currentCellPosition.x - 1, currentCellPosition.y);
            if (_cellsInGrid.TryGetValue(westPos, out FlowVector westCell))
                cell.Value.AddNeighbour(westCell);
        }
    }

    private void GenerateIntegrationField()
    {
        while (_openCells.Count != 0)
        {
            FlowVector selectedCell = _openCells.Peek();
            _openCells.Dequeue();
            _visitedCells.Add(selectedCell);
            
            foreach (var neighbour in selectedCell.NeighbourCells)
            {
                //Cell is unpassable (wall)
                if (neighbour.Cost == MAX_COST) continue;
                
                //Cell is already visited
                if (neighbour.Visited) continue;
                neighbour.Visited = true;

                int newNeighbourDistance = selectedCell.Cost + 1;
                Debug.Log($"New distance neighbour: {newNeighbourDistance}");

                if (newNeighbourDistance < neighbour.Cost)
                {
                    neighbour.Cost = newNeighbourDistance;
                    neighbour.PreviousCell = selectedCell;
                }

                Debug.Log($"Neighbour cost after check: {neighbour.Cost}");

                _openCells.Enqueue(neighbour);
            }
        }
    }

    private void ResetIntegrationValues()
    {
        _visitedCells.Clear();
        _openCells.Clear();
        
        foreach (KeyValuePair<Vector2, FlowVector> vector in _cellsInGrid)
        {
            vector.Value.Cost = CheckCellTraversability(vector.Value.Position, vector.Value.Size);
        }
    }

    private int CheckCellTraversability(Vector2 position, Vector2 pOffsettedSize)
    {
        ContactFilter2D newFilter = new();
        newFilter.NoFilter();

        List<Collider2D> collisions = new();

        int count = Physics2D.OverlapBox(position, pOffsettedSize, 0f, newFilter, collisions);
        if (count != 0)
        {
            foreach (Collider2D foundCollider in collisions)
            {
                if (foundCollider.CompareTag("Cell")) 
                    continue;
                return MAX_COST;
            }
        }

        return int.MaxValue;
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
                
                ResetIntegrationValues();
                _currentCell.Cost = 0;
                _openCells.Enqueue(_currentCell);
                GenerateIntegrationField();
                GenerateFlowField();
            }
        }
    }
}