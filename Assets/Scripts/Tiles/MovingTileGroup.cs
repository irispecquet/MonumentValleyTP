using System;
using System.Collections.Generic;
using DG.Tweening;
using Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tiles
{
    public class MovingTileGroup : TileGroup
    {
        [SerializeField] private Tile[] _tilesToRefresh;
        [SerializeField] private Vector3 _movingDirection;
        [SerializeField] private float _rayCollisionDistance;
        [SerializeField] private bool _showRayDebug;
        [SerializeField] private List<Vector3> _snapPositions = new List<Vector3>();
        [SerializeField] private LayerMask _collideAgainstLayer;

        private bool _isMoving;
        private Tile _occupiedTile;
        
        private void Start()
        {
            Initialize();
        }

        protected override void OnToolBeginClick(Vector3 mousePosition)
        {
            _isDragging = true;
            _initialMousePosition = mousePosition;
            _isMoving = true;
            
            foreach (Tile tile in _tiles)
            {
                if (tile.IsOccupied)
                    _occupiedTile = tile;
            }
        }

        protected override void OnToolClick(Vector3 mousePosition)
        {
            if (!_isDragging) return;

            Vector3 screenPoint = new(mousePosition.x, mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z);
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(screenPoint);
            Vector3 targetPosition = Vector3.Scale(mouseWorldPosition, _movingDirection) + Vector3.Scale(transform.position, Vector3.one - _movingDirection);
            Vector3 moveDirection = targetPosition - transform.position;
            float distance = moveDirection.magnitude;

            if(_showRayDebug)
                Debug.DrawLine(transform.position, transform.position + moveDirection.normalized * (distance + _rayCollisionDistance), Color.red);
            
            if (!Physics.Raycast(transform.position, moveDirection.normalized, distance + _rayCollisionDistance, _collideAgainstLayer))
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);
        }

        private Vector3 GetClosestPosition(Vector3 originPosition)
        {
            Vector3 closestPosition = _snapPositions[^1];
            
            foreach (Vector3 position in _snapPositions)
            {
                if (Vector3.Distance(originPosition, position) < Vector3.Distance(originPosition, closestPosition))
                    closestPosition = position;
            }

            return closestPosition;
        }

        protected override void OnToolEndClick()
        {
            _isDragging = false;

            transform.DOMove(GetClosestPosition(transform.position), 0.2f).OnComplete(() =>
            {
                foreach (Tile tile in _tiles)
                    tile.FindNeighbours();

                foreach (Tile tile in _tilesToRefresh)
                    tile.FindNeighbours();

                _isMoving = false;
                _occupiedTile = null;
            });
        }

        private void Update()
        {
            if (_isMoving && _occupiedTile != null)
                GameCore.Instance.Player.transform.position = _occupiedTile.PlayerPosition.position;
        }

        [Button]
        public void AddSnappingPosition()
        {
           _snapPositions.Add(transform.position);
        }

        private void OnDestroy()
        {
            Destroy();
        }
    }
}