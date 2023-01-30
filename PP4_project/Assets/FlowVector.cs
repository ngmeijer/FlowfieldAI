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
    public Vector2 Direction = Vector2.right;
    public Vector2 Position;
    public Vector2 Size;
    public int Index;
    public int Cost = 0;

    public bool Visited = false;

    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Sprite _selectedIcon;
    [SerializeField] private Sprite _directionIcon;

    [SerializeField] private SpriteRenderer _background;

    private void Start()
    {
        float angle = Vector2.Angle(transform.rotation.eulerAngles, Direction);
        transform.Rotate(Vector3.forward, angle);
    }

    public void OnSelectCell()
    {
        _renderer.sprite = _selectedIcon;
    }

    public void Initialize(Vector2 pPosition, Vector2 pSize, int pIndex)
    {
        Position = pPosition;
        transform.position = pPosition;
        Index = pIndex;
        Size = pSize;
        _collider.size = pSize;
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
    }
}
