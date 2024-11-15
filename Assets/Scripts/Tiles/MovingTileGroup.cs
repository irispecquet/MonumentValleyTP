using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tiles
{
    public class MovingTileGroup : TileGroup
    {
        [SerializeField] private Vector3 _movingDirection;
        [SerializeField] private float _rayCollisionDistance;
        [SerializeField] private List<Vector3> _snapPositions = new List<Vector3>();

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
            
            _occupiedTile = GetOccupiedTile();
        }

        protected override void OnToolClick(Vector3 mousePosition)
        {
            if (!_isDragging) return;

            Vector3 screenPoint = new(mousePosition.x, mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z);
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(screenPoint);
            Vector3 targetPosition = Vector3.Scale(mouseWorldPosition, _movingDirection) + Vector3.Scale(transform.position, Vector3.one - _movingDirection);
            Vector3 moveDirection = targetPosition - transform.position;
            float distance = moveDirection.magnitude;

            if (!Physics.Raycast(transform.position, moveDirection.normalized, distance + _rayCollisionDistance))
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
                GameCore.Instance.RefreshTilesNeighbours();
                
                foreach (Tile tile in _tiles)
                    tile.FindNeighbours();

                _isMoving = false;
            });
        }

        private void Update()
        {
            if (_isMoving && _occupiedTile != null)
                GameCore.Instance.Player.transform.position = _occupiedTile.PlayerPosition.position;
        }

        private Tile GetOccupiedTile()
        {
            foreach (Tile tile in _tiles)
            {
                if (tile.IsOccupied)
                    return tile;
            }
            
            return null;
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