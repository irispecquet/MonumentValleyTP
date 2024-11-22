using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Tiles
{
    public class RotatingTileGroup : TileGroup
    {
        [SerializeField] private Dictionary<CardinalDirection, DynamicNeighbourTileContainer> _endTiles = new();
        [SerializeField] private Transform _pivotPoint;
        [SerializeField] private Vector3 _rotation;
        [SerializeField] private float _automaticRotationDuration = 0.5f;
        
        private CardinalDirection _currentDirection;
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
            SetCurrentDirection(GetClosestCardinalDirection());
            Initialize();
        }

        #region ROTATION EVENTS

        protected override void OnToolBeginClick(Vector3 mousePosition)
        {
            if (!CanRotate())
                return;

            _isDragging = true;
            _initialMousePosition = mousePosition;
        }

        protected override void OnToolClick(Vector3 mousePosition)
        {
            if (!_isDragging) return;
            
            Rotate(mousePosition);
        }

        protected override void OnToolEndClick()
        {
            EndRotation();
            _isDragging = false;
        }

        #endregion // ROTATION EVENTS

        #region ROTATION

        private void Rotate(Vector3 mousePosition)
        {
            Vector3 currentMousePosition = mousePosition;
            Vector3 mouseDelta = currentMousePosition - _initialMousePosition;

            float rotationAngle = mouseDelta.x * _speed;
            
            _pivotPoint.Rotate(_rotation, rotationAngle * Time.deltaTime);
            _initialMousePosition = currentMousePosition;
        }

        private void EndRotation()
        {
            _rotationTween?.Complete();
            
            CardinalDirection newCardinalDirection = GetClosestCardinalDirection();
            Vector3 targetValue = _rotation * _rotationAngles[newCardinalDirection];

            _rotationTween = _pivotPoint.DOLocalRotate(targetValue, _automaticRotationDuration).OnComplete(() => { SetCurrentDirection(newCardinalDirection); });
        }

        private void SetCurrentDirection(CardinalDirection newCardinalDirection)
        {
            if (_currentDirection == newCardinalDirection)
                return;
            
            _currentDirection = newCardinalDirection;

            foreach (Tile tile in _tiles)
            {
                Vector3 rotationAngle = _pivotPoint.eulerAngles * -1;
                tile.transform.DOLocalRotate(rotationAngle, 0);
            }

            foreach (Tile tile in _tiles)
                tile.FindNeighbours();

            if (_endTiles.Count <= 0) 
                return;
            
            if (!_endTiles.TryGetValue(_currentDirection, out DynamicNeighbourTileContainer dynamicNeighborTiles)) 
                return;
                
            dynamicNeighborTiles.Turn();
        }

        public void Turn(bool clockwise)
        {
            _rotationTween?.Complete();
            
            CardinalDirection newCardinalDirection;
            CardinalDirection[] directionValues = (CardinalDirection[])Enum.GetValues(typeof(CardinalDirection));
            
            if (clockwise)
            {
                newCardinalDirection = (CardinalDirection)(((int)_currentDirection + 1) % directionValues.Length);
            }
            else
            {
                int newDirectionIndex = ((int)_currentDirection - 1 + directionValues.Length) % directionValues.Length;
                newCardinalDirection = (CardinalDirection)newDirectionIndex;
            }

            Vector3 targetValue = _rotation * _rotationAngles[newCardinalDirection];
            _rotationTween = _pivotPoint.DOLocalRotate(targetValue, _automaticRotationDuration).OnComplete(() => { SetCurrentDirection(newCardinalDirection); });
        }

        #endregion // ROTATION

        [Button]
        public void RotateClockwiseInEditor()
        {
            _pivotPoint.Rotate(_rotation, 90);
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
            float currentAngle = 0;

            currentAngle = _rotation.x >= 1 ? _pivotPoint.eulerAngles.x :
                           _rotation.z >= 1 ? _pivotPoint.eulerAngles.z :
                           _rotation.y >= 1 ? _pivotPoint.eulerAngles.y : currentAngle;

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
            Destroy();
        }
    }
    
    [Serializable]
    public class DynamicNeighbourTileContainer
    {
        [SerializeField] private UnityEvent _turnEvent; 
        [SerializeField] private List<DynamicNeighborTile> _tiles = new List<DynamicNeighborTile>();
        
        public void Turn()
        {
            foreach (DynamicNeighborTile tile in _tiles)
                tile.SetNeighbourTile();
            
            _turnEvent?.Invoke();
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