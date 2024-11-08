using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tiles
{
    public class RotatingTileGroup : TileGroup
    {
        [SerializeField] private Dictionary<CardinalDirection, List<DynamicNeighborTile>> _endTiles = new();
        [SerializeField] private Transform _pivotPoint;
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private float _automaticRotationSpeed = 1f;
        [SerializeField] private RotatingTileTool _rotatingTileTool;
        
        private CardinalDirection _currentDirection;
        private Vector3 _initialMousePosition;
        private bool _isDragging;
        private Tween _rotationTween;

        private Dictionary<CardinalDirection, float> _rotationAngles = new()
        {
            { CardinalDirection.North, 0 },
            { CardinalDirection.East, 90 },
            { CardinalDirection.South, 180 },
            { CardinalDirection.West, 270 },
        };

        private void Start()
        {
            _tiles = new List<Tile>(GetComponentsInChildren<Tile>());

            SetCurrentDirection(GetClosestCardinalDirection());
            
            if (_rotatingTileTool != null)
            {
                _rotatingTileTool.BeginClickEvent += OnToolBeginClick;
                _rotatingTileTool.ClickEvent += OnToolClick;
                _rotatingTileTool.EndClickEvent += OnToolEndClick;
            }
        }

        #region ROTATION EVENTS

        private void OnToolBeginClick(Vector3 mousePosition)
        {
            if (!CanRotate())
                return;
            
            _initialMousePosition = mousePosition;
            _isDragging = true;
        }

        private void OnToolClick(Vector3 mousePosition)
        {
            if (!_isDragging) return;

            Rotate(mousePosition);
        }

        private void OnToolEndClick()
        {
            _isDragging = false;
            EndRotation(_automaticRotationSpeed);
        }

        #endregion // ROTATION EVENTS

        #region ROTATION

        private void Rotate(Vector3 mousePosition)
        {
            Vector3 currentMousePosition = mousePosition;
            Vector3 mouseDelta = currentMousePosition - _initialMousePosition;

            float rotationAngle = mouseDelta.x * _rotationSpeed;
            
            _pivotPoint.Rotate(Vector3.up, rotationAngle * Time.deltaTime);
            _initialMousePosition = currentMousePosition;
        }

        private void EndRotation(float tweenSpeed)
        {
            _rotationTween?.Complete();
            
            CardinalDirection newCardinalDirection = GetClosestCardinalDirection();
            Vector3 targetValue = Vector3.up * _rotationAngles[newCardinalDirection];
            
            _rotationTween = _pivotPoint.DORotate(targetValue, tweenSpeed).OnComplete(() => { SetCurrentDirection(newCardinalDirection); });
        }

        private void SetCurrentDirection(CardinalDirection newCardinalDirection)
        {
            if (_currentDirection == newCardinalDirection)
                return;
            
            _currentDirection = newCardinalDirection;

            GameCore.Instance.RefreshTilesNeighbours();

            if (_endTiles.Count <= 0) 
                return;
            
            if (!_endTiles.TryGetValue(_currentDirection, out List<DynamicNeighborTile> dynamicNeighborTiles)) 
                return;
                
            foreach (DynamicNeighborTile tile in dynamicNeighborTiles)
                tile.SetNeighbourTile();
        }

        #endregion // ROTATION

        [Button]
        public void RotateClockwise()
        {
            _pivotPoint.Rotate(Vector3.up, 90);
        }

        private bool CanRotate()
        {
            foreach (Tile tile in _tiles)
            {
                if (tile.IsOccupied)
                    return false;
            }

            return true;
        }

        private CardinalDirection GetClosestCardinalDirection()
        {
            float currentAngle = _pivotPoint.eulerAngles.y;
            float minDistance = 360;
            CardinalDirection cardinalDirection = CardinalDirection.North;

            foreach ((CardinalDirection dir, float angle) in _rotationAngles)
            {
                float distance = Mathf.Abs(Mathf.DeltaAngle(currentAngle, angle));

                if (distance < minDistance)
                {
                    minDistance = distance;
                    cardinalDirection = dir;
                }
            }

            return cardinalDirection;
        }


        private void OnDestroy()
        {
            if (_rotatingTileTool != null)
            {
                _rotatingTileTool.BeginClickEvent -= OnToolBeginClick;
                _rotatingTileTool.ClickEvent -= OnToolClick;
                _rotatingTileTool.EndClickEvent -= OnToolEndClick;
            }
        }
    }

    [Serializable]
    public class DynamicNeighborTile
    {
        [field:SerializeField] public Tile Tile { get; private set; }
        [field:SerializeField] public Vector3 NeighbourDirection { get; private set; }
        [field:SerializeField] public Tile Neighbour { get; private set; }
        [field:SerializeField] public Vector3 TileDirection { get; private set; }

        public void SetNeighbourTile()
        {
            Tile.FindNeighbours();
            Tile.SetNeighbour(NeighbourDirection, Neighbour);

            if (Neighbour != null)
            {
                Neighbour.FindNeighbours();
                Neighbour.SetNeighbour(TileDirection, Tile);
            }
        }
    }

    public enum CardinalDirection
    {
        North = 1 << 0,
        East = 1 << 1,
        South = 1 << 2,
        West = 1 << 3
    }
}