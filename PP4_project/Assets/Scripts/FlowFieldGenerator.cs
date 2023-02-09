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

    public float RotationMultiplier = 90f;

    private FlowVector _currentCell;
    private Queue<FlowVector> _openCells = new Queue<FlowVector>();
    private List<FlowVector> _visitedCells = new List<FlowVector>();
    private Dictionary<Vector2, FlowVector> _cellsInGrid = new Dictionary<Vector2, FlowVector>();

    private void Awake()
    {
        _cam = Camera.main;

        GenerateGrid();
    }

    private void Update()
    {
        CheckOnClickCell();
    }

    public Vector3 LookUpDirectionFromCell(Vector2 pAgentPosition)
    {
        
        if (_cellsInGrid.ContainsKey(pAgentPosition))
        {
            
        }
        
        return Vector3.zero;
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
                Vector2Int index = new Vector2Int((int)x, (int)y);
                FlowVector newVector = Instantiate(_flowVectorPrefab, this.transform);
                newVector.transform.name = $"Cell {index}";

                int cost = CheckCellTraversability(position, size - innerOffset);

                newVector.Initialize(index, position, size, cost);
                _cellsInGrid.Add(position, newVector);
            }
        }

        foreach (var cell in _cellsInGrid)
        {
            Vector2 currentCellPosition = cell.Value.Position;
            
            //N
            GetNeighbour(new Vector2(currentCellPosition.x, currentCellPosition.y + 1), cell.Value);
            //N-E
            GetNeighbour(new Vector2(currentCellPosition.x + 1, currentCellPosition.y + 1), cell.Value);
            //E
            GetNeighbour(new Vector2(currentCellPosition.x + 1, currentCellPosition.y), cell.Value);
            //S-E
            GetNeighbour(new Vector2(currentCellPosition.x + 1, currentCellPosition.y - 1), cell.Value);
            //S
            GetNeighbour(new Vector2(currentCellPosition.x, currentCellPosition.y - 1), cell.Value);
            //S-W
            GetNeighbour(new Vector2(currentCellPosition.x - 1, currentCellPosition.y - 1), cell.Value);
            //W
            GetNeighbour(new Vector2(currentCellPosition.x - 1, currentCellPosition.y), cell.Value);
            //N-W
            GetNeighbour(new Vector2(currentCellPosition.x - 1, currentCellPosition.y + 1), cell.Value);
        }
    }

    private void GetNeighbour(Vector2 pPosition, FlowVector pCurrentCell)
    {
        if (_cellsInGrid.TryGetValue(pPosition, out FlowVector northWestCell))
            pCurrentCell.AddNeighbour(northWestCell);
    }

private void GenerateIntegrationField()
    {
        while (_openCells.Count > 0)
        {
            FlowVector selectedCell = _openCells.Peek();
            _openCells.Dequeue();
            _visitedCells.Add(selectedCell);
            
            foreach (var neighbour in selectedCell.NeighbourCells)
            {
                if (IsUnpassable(neighbour) || IsVisited(neighbour))
                {
                    continue;
                }

                UpdateNeighbourData(selectedCell, neighbour);
                _openCells.Enqueue(neighbour);
            }
        }
    }

private bool IsUnpassable(FlowVector cell)
{
    return Math.Abs(cell.Cost - MAX_COST) < 0.05f;
}

private bool IsVisited(FlowVector cell)
{
    return cell.Visited;
}

private void UpdateNeighbourData(FlowVector selectedCell, FlowVector neighbour)
{
    neighbour.Visited = true;
    float relativeDistanceToNeighbour = Vector3.Distance(selectedCell.Position, neighbour.Position);
    float newNeighbourDistance = selectedCell.Cost + relativeDistanceToNeighbour;
    if (newNeighbourDistance < neighbour.Cost)
    {
        neighbour.Cost = newNeighbourDistance;
        neighbour.PreviousCell = selectedCell;
    }
    neighbour.AssignHeatIntensity();
}


    private void ResetIntegrationValues()
    {
        _visitedCells.Clear();
        _openCells.Clear();
        
        foreach (KeyValuePair<Vector2, FlowVector> vector in _cellsInGrid)
        {
            vector.Value.ResetCell();
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
                if (foundCollider.CompareTag("Agent"))
                    continue;
                return MAX_COST;
            }
        }

        return int.MaxValue;
    }

    private void GenerateFlowField()
    {
        foreach (var currentLoopingCell in _cellsInGrid)
        {
            FlowVector currentBestNeighbour = FindBestNeighbour(currentLoopingCell.Value);
            Vector3 targetDirection = GetTargetDirection(currentLoopingCell.Value, currentBestNeighbour);
            Quaternion newRotation = CalculateRotation(targetDirection);
            RotateCellTowardsNeighbour(currentLoopingCell.Value, newRotation, currentBestNeighbour);
        }
    }

    private FlowVector FindBestNeighbour(FlowVector currentCell)
    {
        FlowVector currentBestNeighbour = null;
        foreach (var neighbour in currentCell.NeighbourCells)
        {
            if (currentBestNeighbour == null)
            {
                currentBestNeighbour = neighbour;
                continue;
            }

            if (neighbour.Cost < currentBestNeighbour.Cost)
            {
                currentBestNeighbour = neighbour;
            }
        }

        return currentBestNeighbour;
    }

    
    private Vector3 GetTargetDirection(FlowVector currentCell, FlowVector bestNeighbour)
    {
        return bestNeighbour.transform.position - currentCell.transform.position;
    }

    private Quaternion CalculateRotation(Vector3 targetDirection)
    {
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg + RotationMultiplier;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void RotateCellTowardsNeighbour(FlowVector currentCell, Quaternion newRotation, FlowVector bestNeighbour)
    {
        currentCell.RotateTowards(newRotation, bestNeighbour);
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
                if (vector.Cost == MAX_COST) return;
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