using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField, FoldoutGroup("References")]
    private Animator _animator;

    [SerializeField] private bool _snapPlayerToTile;
    [SerializeField] private float _downRayDistance;
    [SerializeField] private float _forwardRayDistance;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotationDuration;

    private Tile _currentTile;
    private PathFinder _pathFinder;
    private bool _isMoving;
    private Tween _rotationTween;
    private List<Tile> _targetPath;
    private (Tile tile, int index) _targetTile;
    private Action<Vector3> _reachTargetTileEvent;
    
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsClimbing = Animator.StringToHash("IsClimbing");
    private static readonly int IsAgainstWall = Animator.StringToHash("IsAgainstTile");

    #region UNITY METHODS

    private void Awake()
    {
        _pathFinder = new PathFinder();
    }

    private void Start()
    {
        if (IsOnTile(out Tile tile))
        {
            _currentTile = tile;

            if (_snapPlayerToTile)
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
        if (_isMoving)
            Move();
    }

    #endregion

    #region TILE_CHECK

    private Vector3 GetOrigin()
    {
        return transform.position + new Vector3(0, 1, -1) * 0.1f;
    }

    private bool IsOnTile(out Tile tile)
    {
        tile = null;
        bool isOnSomething = Physics.Raycast(GetOrigin(), -transform.up, out RaycastHit hit, _downRayDistance);

        if (isOnSomething)
            return hit.collider.TryGetComponent(out tile);

        return false;
    }

    private bool IsAgainstTile(out Tile tile)
    {
        tile = null;
        bool isAgainstSomething = Physics.Raycast(GetOrigin(), transform.forward, out RaycastHit hit, _forwardRayDistance);

        if (isAgainstSomething)
            return hit.collider.TryGetComponent(out tile);

        return false;
    }

    #endregion // TILE_CHECK

    #region MOVEMENT

    public void BeginMove(Tile targetTile)
    {
        List<Tile> path = _pathFinder.GetPath(_currentTile, targetTile);

        if (path.Count <= 1)
            return;

        int startingIndex = 1;

        _targetPath = new List<Tile>(path);
        _targetTile = (path[startingIndex], startingIndex);

        _animator.SetBool(IsWalking, !_targetTile.tile.IsLadder);
        _animator.SetBool(IsClimbing, _targetTile.tile.IsLadder);

        Turn(path[startingIndex].PlayerLookAt.position);
        _reachTargetTileEvent += Turn;

        _isMoving = true;
    }

    private void Move()
    {
        if (_targetTile.index >= _targetPath.Count)
            return;

        Vector3 targetPosition = _targetTile.tile.PlayerPosition.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, _moveSpeed * Time.fixedDeltaTime);

        if (!(Vector3.Distance(transform.position, targetPosition) < 0.05f)) 
            return;
        
        int index = _targetTile.index + 1;

        if (index < _targetPath.Count)
        {
            _targetTile = (_targetPath[index], index);
            _animator.SetBool(IsClimbing, _targetTile.tile.IsLadder);
            _reachTargetTileEvent?.Invoke(_targetTile.tile.PlayerLookAt.position);
        }
        else
            EndMoving();
    }

    private void Turn(Vector3 lookAtPoint)
    {
        Vector3 direction = lookAtPoint - transform.position;
        direction.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        _rotationTween?.Complete();
        _rotationTween = transform.DOLocalRotateQuaternion(targetRotation, _rotationDuration);
    }

    private void EndMoving()
    {
        _animator.SetBool(IsAgainstWall, IsAgainstTile(out _));
        _animator.SetBool(IsWalking, false);
        _animator.SetBool(IsClimbing, false);
            
        _reachTargetTileEvent -= Turn;
        _isMoving = false;

        _targetPath = null;
        _targetTile = (null, 0);
    }

    #endregion // MOVEMENT

    private void RefreshCurrentTile()
    {
        if (!IsOnTile(out Tile tile) && !IsAgainstTile(out tile))
            return;

        if (tile != _currentTile)
        {
            _currentTile.OnBeingCurrentTile(false);
            tile.OnBeingCurrentTile(true);
        }

        _currentTile = tile;
    }

    #region DEBUG

    private void OnDrawGizmos()
    {
        Gizmos.color = IsOnTile(out _) ? Color.green : Color.red;
        Vector3 raycastOrigin = GetOrigin();
        Gizmos.DrawLine(raycastOrigin, raycastOrigin + -transform.up * _downRayDistance);
        Gizmos.color = IsAgainstTile(out _) ? Color.green : Color.red;
        Gizmos.DrawLine(raycastOrigin, raycastOrigin + transform.forward * _forwardRayDistance);
    }

    #endregion
}