using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Tile : SerializedMonoBehaviour
{
    [SerializeField] private float _neighbourRayLength = 1.2f;
    
    [SerializeField, FoldoutGroup("Tween")] private float _punchPositionForce;
    [SerializeField, FoldoutGroup("Tween")] private float _punchPositionDuration;
    
    [SerializeField, FoldoutGroup("References")] private MeshRenderer _meshRenderer;
    [SerializeField, FoldoutGroup("References")] private Material _highlightedMaterial;
    [SerializeField, FoldoutGroup("References")] private Transform _playerPosition;
    [SerializeField, FoldoutGroup("References")] private Collider _collider;
    [SerializeField, FoldoutGroup("References")] private bool _chooseNeighbours;
    [SerializeField, ShowIf(nameof(_chooseNeighbours)), FoldoutGroup("References"), 
     Tooltip("If you want to attribute specific neighbours to specific directions, you can do it here. All the null values will be filled automatically.")]
    private Dictionary<Vector3, Tile> _neighbors = new Dictionary<Vector3, Tile>();

    public Transform PlayerPosition => _playerPosition;
    public Dictionary<Vector3, Tile> Neighbors => _neighbors;

    public bool IsLadder => _isLadder;

    private Material _initTileMaterial;
    private bool _isLadder;

    private void Start()
    {
        FindNeighbours();
        SetLadder();
        _initTileMaterial = _meshRenderer.sharedMaterial;
    }

    private void SetLadder()
    {
        if(_neighbors.TryGetValue(Vector3.up, out _))
            _isLadder = true;
    }

    public void RefreshMaterial(bool isCurrent)
    {
        _meshRenderer.sharedMaterial = isCurrent ? _highlightedMaterial : _initTileMaterial;
    }

    #region NEIGHBOURS

    private void FindNeighbours()
    {
        Vector3[] directions = GetDirections();
        foreach (Vector3 direction in directions)
        {
            bool detectBlock = TryFindNeighbour(direction, out RaycastHit hit);

            if (detectBlock)
            {
                Tile tile = hit.collider.GetComponent<Tile>();
                _neighbors.TryAdd(direction, tile);
            }
        }
    }

    private bool TryFindNeighbour(Vector3 direction, out RaycastHit hit)
    {
        float maxDistance = _collider.bounds.extents.y * _neighbourRayLength;
        bool detectBlock = Physics.Raycast(transform.position, direction, out hit, maxDistance);
        Debug.DrawLine(transform.position, transform.position + direction * maxDistance, detectBlock ? Color.green : Color.red);

        return detectBlock;
    }

    private Vector3[] GetDirections()
    {
        Vector3[] directions =
        {
            transform.forward,
            transform.right,
            -transform.right,
            -transform.forward,
            transform.up,
            -transform.up,
        };

        return directions;
    }

    #endregion // NEIGHBOURS

    private Tween _punchPositionTween;

    private void OnMouseDown()
    {
        GameCore.Instance.Player.BeginMove(this);

        _punchPositionTween?.Complete();
        _punchPositionTween = transform.DOPunchPosition(Vector3.up * -_punchPositionForce, _punchPositionDuration, 0, 0);
    }
}