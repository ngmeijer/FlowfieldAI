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
    public Vector2 Direction = Vector2.right;
    public Vector2 Position;
    public Vector2 Size;
    public int Cost = int.MaxValue;
    public FlowVector PreviousCell;
    [Space(10)]
    public List<FlowVector> NeighbourCells = new List<FlowVector>();
    public bool Visited;

    [Header("Components")]
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Sprite _selectedIcon;
    [SerializeField] private Sprite _directionIcon;
    [SerializeField] private SpriteRenderer _background;
    
    private void Awake()
    {
        float angle = Vector2.Angle(transform.rotation.eulerAngles, Direction);
        transform.Rotate(Vector3.forward, angle);
    }

    public void OnSelectCell()
    {
        _renderer.sprite = _selectedIcon;
    }

    public void Initialize(Vector2 pPosition, Vector2 pSize, int pCost)
    {
        Position = pPosition;
        transform.position = pPosition;
        Cost = pCost;
        Size = pSize;
        _collider.size = pSize;

        if (Cost == FlowFieldGenerator.MAX_COST) _renderer.sprite = null;
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

    public void SetVisited()
    {
        _background.color = new Color(255, 0, 0, 128);
    }

    public void OnGUI()
    {
        Handles.Label(transform.position, $"{Cost}");
        Handles.Label(transform.position - new Vector3(0, 0.15f), $"{Position}");
    }
}
