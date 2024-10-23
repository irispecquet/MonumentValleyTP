using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool _snapPlayerToTile;
    [SerializeField] private float _maxTileRaycastDistance;
    [SerializeField] private float _speed;
    
    private Tile _currentTile;
    private Collider _collider;
    private PathFinder _pathFinder;
    private bool _isMoving;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _pathFinder = new PathFinder();
    }

    private void Start()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, _maxTileRaycastDistance))
        {
            if (hit.collider.TryGetComponent(out Tile tile))
            {
                _currentTile = tile;
                transform.position = _currentTile.PlayerPosition.position;
            }
            else
            {
                Debug.LogError($"The player must be on a tile, not {hit.collider.gameObject.name}");
            }
        }
        else
        {
            Debug.LogError($"The player must be on a tile!");
        }
    }

    private Tween _moveTween;

    public void Move(Tile targetTile)
    {
        if (_isMoving)
            return;
        
        List<Tile> path = _pathFinder.GetPath(_currentTile, targetTile);

        if (path.Count <= 0)
            return;

        Sequence moveSequence = DOTween.Sequence();
        
        foreach (Tile tile in path)
        {
            if (tile == _currentTile)
                continue;
                
            Vector3 target = tile.PlayerPosition.position;
            
            moveSequence.Append(transform.DOMove(target, _speed).SetEase(Ease.Linear));
            moveSequence.Join(transform.DOLookAt(target, _speed).SetEase(Ease.Linear));
            
            _currentTile = tile;
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Physics.Raycast(transform.position, -transform.up, _maxTileRaycastDistance) ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, transform.position + -transform.up * _maxTileRaycastDistance);
        }
    }
}