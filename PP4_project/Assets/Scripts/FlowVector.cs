using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class FlowVector : MonoBehaviour
{
    [Header("Properties")] 
    public Vector2Int Index;
    public Vector2 Direction;
    public Vector2 Position;
    public Vector2 Size;
    public float Cost = int.MaxValue;
    public FlowVector PreviousCell;
    [Space(10)]
    public List<FlowVector> NeighbourCells = new List<FlowVector>();
    public bool Visited;
    private FlowVector _bestNeighbour;

    [Header("Components")]
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Sprite _selectedIcon;
    [SerializeField] private Sprite _directionIcon;
    [SerializeField] private SpriteRenderer _background;
    
    public void OnSelectCell()
    {
        _renderer.sprite = _selectedIcon;
    }

    public void Initialize(Vector2Int pIndex, Vector2 pPosition, Vector2 pSize, int pCost)
    {
        Index = pIndex;
        Position = pPosition;
        transform.position = pPosition;
        Cost = pCost;
        Size = pSize;
        _collider.size = pSize;

        if (Cost == FlowFieldGenerator.MAX_COST) _renderer.sprite = null;
    }

    public void AssignHeatIntensity()
    {
        float redValue = 1f / Cost;
        _background.color = new Color(redValue,0,0,1);
    }

    public void RotateTowards(Quaternion pRotateToCell, FlowVector pBestNeighbour)
    {
        _bestNeighbour = pBestNeighbour;
        _renderer.transform.rotation = pRotateToCell;
    }

    public void ResetCell()
    {
        Visited = false;
        _background.color = new Color(0, 0, 0, 0.5f);
    }

    public void AddNeighbour(FlowVector pNeighbourCell)
    {
        NeighbourCells.Add(pNeighbourCell);
    }

    public void RemoveNeighbour(FlowVector pNeighbourCell)
    {
        NeighbourCells.Remove(pNeighbourCell);
    }

    public void OnDeselectCell()
    {
        _renderer.sprite = _directionIcon;
    }

    // public void OnGUI()
    // {
    //     Handles.color = Color.white;
    //     string costValue = Cost.ToString();
    //     if (Cost == int.MaxValue || Cost == 255)
    //         costValue = "MAX";
    //     //Handles.Label(transform.position - new Vector3(0.4f, 0.3f), $"Cost: {costValue}");
    //     //Handles.Label(transform.position - new Vector3(0.4f, 0.15f), $"Index{Index}");
    //     //Handles.Label(transform.position - new Vector3(0.4f, -0.45f), $"Position\n{Position}");
    //     //if(_bestNeighbour != null)
    //     //    Handles.Label(transform.position - new Vector3(0.4f, -0.45f), $"Closest: \n{_bestNeighbour.Index}");
    // }
}
