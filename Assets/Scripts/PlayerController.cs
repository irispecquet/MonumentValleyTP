using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private bool _snapPlayerToTile;
    [SerializeField] private float _maxTileRaycastDistance;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotationDuration;
    
    private Tile _currentTile;
    private PathFinder _pathFinder;
    private bool _isMoving;
    private Tween _rotationTween;
    private List<Tile> _targetPath;
    private (Tile tile, int index) _targetTile;
    private Action<Vector3> _reachTargetTileEvent;

    #region UNITY METHODS
    
    private void Awake()
    {
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

        private void Update()
    {
        RefreshCurrentTile();
    }
    
    private void FixedUpdate()
    {
        if(_isMoving)
            Move();
    }

    #endregion
    
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

    public void BeginMove(Tile targetTile)
    {
        List<Tile> path = _pathFinder.GetPath(_currentTile, targetTile);
        
        if (path.Count <= 1)
            return;

        int startingIndex = 1;
        
        _targetPath = new List<Tile>(path);
        this._targetTile = (path[startingIndex], startingIndex); 
        _animator.SetBool($"IsWalking", true);
        Turn(path[startingIndex].PlayerPosition.position);
        _reachTargetTileEvent += Turn;
        _isMoving = true;
    }
    
    private void Move()
    {
        if (_targetTile.index < _targetPath.Count)
        {
            Vector3 targetPosition = _targetTile.tile.PlayerPosition.position;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _moveSpeed * Time.fixedDeltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                int index = _targetTile.index + 1;

                if (index < _targetPath.Count)
                {
                    _targetTile = (_targetPath[index], index);
                    _reachTargetTileEvent?.Invoke(_targetTile.tile.PlayerPosition.position);
                }
                else
                    EndMoving();                    
            }
        }
    }

    private void Turn(Vector3 lookAtPoint)
    {
        _rotationTween?.Complete();
        _rotationTween = transform.DOLookAt(lookAtPoint, _rotationDuration);
    }

    private void EndMoving()
    {
        _animator.SetBool($"IsWalking", false);
        _reachTargetTileEvent -= Turn;
        _isMoving = false;
        
        _targetPath = null;
        _targetTile = (null, 0); 
    }

    private void RefreshCurrentTile()
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

    #region DEBUG

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = IsOnTile(out _) ? Color.green : Color.red;
            Vector3 raycastOrigin = GetRaycastOrigin();
            Gizmos.DrawLine(raycastOrigin, raycastOrigin + -transform.up * _maxTileRaycastDistance);
        }
    }

    #endregion
}