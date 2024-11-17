using System;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

public class Tile : SerializedMonoBehaviour
{
    #region VARIABLES

    [SerializeField] private float _horizontalNeighbourRayLength = 1.2f;
    [SerializeField] private float _verticalNeighbourRayLength = 1.2f;
    
    [SerializeField, FoldoutGroup("Tween")] private float _punchPositionForce;
    [SerializeField, FoldoutGroup("Tween")] private float _punchPositionDuration;
    
    [SerializeField, FoldoutGroup("References")] private MeshRenderer[] _meshRenderers;
    [SerializeField, FoldoutGroup("References")] private Transform _neighbourDetectionTransform;
    [SerializeField, FoldoutGroup("References")] private Material _highlightedMaterial;
    [SerializeField, FoldoutGroup("References")] private Transform _playerPosition;
    [SerializeField, FoldoutGroup("References")] private Transform _playerLookAt;
    [SerializeField, FoldoutGroup("References")] private Collider _collider;
    [SerializeField, FoldoutGroup("References")] private bool _chooseNeighbours;
    [SerializeField, ShowIf(nameof(_chooseNeighbours)), FoldoutGroup("References"), 
     Tooltip("If you want to attribute specific neighbours to specific directions, you can do it here. All the null values will be filled automatically.")]
    private Dictionary<Vector3, Tile> _specificInitNeighbours = new();
    [SerializeField, Space] private bool _isFinalTile;
    [SerializeField] private bool _isLadder;

    public bool IsLadder => _isLadder;
    public bool IsOccupied { get; private set; }
    public Transform PlayerPosition => _playerPosition;
    public Dictionary<Vector3, Tile> Neighbours => _neighbours;
    public Transform PlayerLookAt => _playerLookAt;
    public bool PlayerTurn { get; private set; } = true;

    private Material _initTileMaterial;
    private Dictionary<Vector3, Tile> _neighbours;

    #endregion // VARIABLES

    private void Awake()
    {
        FindNeighbours();
        _initTileMaterial = _meshRenderers[0].sharedMaterial;
    }

    public void OnBeingCurrentTile(bool isCurrent)
    {
        IsOccupied = isCurrent;
        
        foreach (MeshRenderer mesh in _meshRenderers)
            mesh.sharedMaterial = isCurrent ? _highlightedMaterial : _initTileMaterial;
        
        if(_isFinalTile)
            GameCore.Instance.GoToNextLevel();
    }

    public void ChangeMaterial(Material material)
    {
        foreach (MeshRenderer mesh in _meshRenderers)
            mesh.sharedMaterial = material;

        _initTileMaterial = material;
    }

    #region NEIGHBOURS

    [Button]
    public void FindNeighbours()
    {
        _neighbours = new Dictionary<Vector3, Tile>(_specificInitNeighbours);
        
        Vector3[] directions = GetDirections();
        foreach (Vector3 direction in directions)
        {
            bool detectBlock = TryFindNeighbour(direction, out RaycastHit hit);

            if (detectBlock)
            {
                Tile tile = hit.collider.GetComponent<Tile>();
                _neighbours.TryAdd(direction, tile);
            }
        }
    }

    private bool TryFindNeighbour(Vector3 direction, out RaycastHit hit)
    {
        float maxDistance = direction.y != 0 ? _verticalNeighbourRayLength : _horizontalNeighbourRayLength;
        bool detectSomething = Physics.Raycast(_neighbourDetectionTransform.position, direction, out hit, maxDistance);
        bool detectBlock = false;
        
        if(detectSomething)
            detectBlock = hit.collider.TryGetComponent(out Tile _);
        
        return detectBlock;
    }
    
    public void SetNeighbour(Vector3 direction, Tile neighbour)
    {
        if (neighbour == null)
            _neighbours.Remove(direction);
            
        _neighbours[direction] = neighbour;
    }

    private Vector3[] GetDirections()
    {
        Vector3 forward = Vector3.forward;
        Vector3 right = Vector3.right;
        Vector3 up = Vector3.up;
        
        Vector3[] directions =
        {
            forward,
            right,
            up,
            -forward,
            -right,
            -up
        };

        return directions;
    }

    #endregion // NEIGHBOURS

    private Tween _punchPositionTween;
    
    private void OnMouseDown()
    {
        GameCore.Instance.Player.BeginMove(this);

        _punchPositionTween?.Complete();
        _punchPositionTween = transform.DOPunchScale(Vector3.one * _punchPositionForce, _punchPositionDuration, 0, 0);
    }

    public void ChangePlayerTurnVar(bool turn)
    {
        PlayerTurn = turn;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3[] directions = GetDirections();
        foreach (Vector3 direction in directions)
        {
            float maxDistance = direction.y != 0 ? _verticalNeighbourRayLength : _horizontalNeighbourRayLength;   
            Gizmos.DrawLine(_neighbourDetectionTransform.position, _neighbourDetectionTransform.position + direction * maxDistance);
        }
    }
}