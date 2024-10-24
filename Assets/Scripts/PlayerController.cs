using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private bool _isMoving;
    
    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _pathFinder = new PathFinder();
    }

    private void Start()
    {
        if(IsOnTile(out Tile tile))
        {
            _currentTile = tile;
            
            if(_snapPlayerToTile)
                transform.position = _currentTile.PlayerPosition.position;
        }
        else
        {
            Debug.LogError($"The player must be on a tile!");
        }
    }
    
    private Vector3 GetRaycastOrigin()
    {
        return transform.position + Vector3.up * 0.1f;
    }

    private bool IsOnTile(out Tile tile)
    {
        tile = null;
        bool isOnSomething = Physics.Raycast(GetRaycastOrigin(), -transform.up, out RaycastHit hit, _maxTileRaycastDistance);

        if (isOnSomething)
            return hit.collider.TryGetComponent(out tile);
        
        return false;
    }

    public void Move(Tile targetTile)
    {
        List<Tile> path = _pathFinder.GetPath(_currentTile, targetTile);

        if (path.Count <= 1)
            return;

        _animator.SetBool($"IsWalking", true);

        _isMoving = true;
        
        foreach (Tile tile in path)
        {
            if (tile == _currentTile)
                continue;
        }
    }
    
    private void EndMoving()
    {
        _animator.SetBool($"IsWalking", false);
        _isMoving = false;
    }

    private void Update()
    {
        if (_isMoving)
        {
            if (!IsOnTile(out Tile tile)) 
                return;
            
            if (tile != _currentTile)
            {
                _currentTile.RefreshMaterial(false);
                tile.RefreshMaterial(true);
            }
                
            _currentTile = tile;
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = IsOnTile(out _) ? Color.green : Color.red;
            Vector3 raycastOrigin = GetRaycastOrigin();
            Gizmos.DrawLine(raycastOrigin, raycastOrigin + -transform.up * _maxTileRaycastDistance);
        }
    }
}