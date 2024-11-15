using System;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

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

    public bool IsLadder { get; private set; }
    public bool IsOccupied { get; private set; }
    public Transform PlayerPosition => _playerPosition;
    public Dictionary<Vector3, Tile> Neighbours => _neighbours;
    public Transform PlayerLookAt => _playerLookAt;
    private Material _initTileMaterial;
    private Dictionary<Vector3, Tile> _neighbours;
    private Vector3 _initialPlayerLookAtPosition;

    #endregion // VARIABLES

    private void Awake()
    {
        _initialPlayerLookAtPosition = _playerLookAt.localPosition;
        FindNeighbours();
        SetLadder();
        _initTileMaterial = _meshRenderers[0].sharedMaterial;
    }

    private void SetLadder()
    {
        if(_neighbours.TryGetValue(Vector3.up, out _))
            IsLadder = true;
    }

    public void OnBeingCurrentTile(bool isCurrent)
    {
        IsOccupied = isCurrent;

        foreach (MeshRenderer mesh in _meshRenderers)
            mesh.sharedMaterial = isCurrent ? _highlightedMaterial : _initTileMaterial;
        
        if(_isFinalTile)
            GameCore.Instance.EndGame();
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
        
        StringBuilder sb = new();
        foreach ((Vector3 dir, Tile neighbour) in _neighbours)
            sb.Append($"{gameObject.name} - {dir} : {neighbour}\n");
        Debug.Log(sb.ToString(), gameObject);
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
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 up = transform.up;
        
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

    public void ChangePlayerLookAtPosition(Transform position)
    {
        _playerLookAt.localPosition = position.position;
    }
    
    public void ResetPlayerLookAtPosition()
    {
        _playerLookAt.localPosition = _initialPlayerLookAtPosition;
    }
    
    private void OnMouseDown()
    {
        GameCore.Instance.Player.BeginMove(this);

        _punchPositionTween?.Complete();
        _punchPositionTween = transform.DOPunchPosition(Vector3.up * -_punchPositionForce, _punchPositionDuration, 0, 0);
    }

    private void OnDrawGizmos()
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