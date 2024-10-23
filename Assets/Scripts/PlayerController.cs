using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private bool _snapPlayerToTile;
    [SerializeField] private float _maxTileRaycastDistance;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotationSpeed;
    
    private Tile _currentTile;
    private Collider _collider;
    private PathFinder _pathFinder;

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

    private Sequence _moveSequence;
    
    public void Move(Tile targetTile)
    {
        _moveSequence?.Kill();
        
        List<Tile> path = _pathFinder.GetPath(_currentTile, targetTile);

        if (path.Count <= 0)
            return;

        _moveSequence = DOTween.Sequence();
        _animator.SetBool($"IsWalking", true);
        
        foreach (Tile tile in path)
        {
            if (tile == _currentTile)
                continue;
                
            _currentTile = tile;
            Vector3 target = tile.PlayerPosition.position;
            
            _moveSequence.Append(transform.DOMove(target, _moveSpeed).SetEase(Ease.Linear));
            _moveSequence.Join(transform.DOLookAt(target, _rotationSpeed).SetEase(Ease.Linear));
        }
        
        _moveSequence.OnComplete(() => _animator.SetBool($"IsWalking", false));
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