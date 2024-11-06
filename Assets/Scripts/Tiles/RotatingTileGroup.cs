using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Tiles
{
    public class RotatingTileGroup : TileGroup
    {
        [SerializeField] private Dictionary<CardinalDirection, List<DynamicNeighborTile>> _endTiles = new Dictionary<CardinalDirection, List<DynamicNeighborTile>>();
        [SerializeField] private Transform _pivotPoint;
        [SerializeField] private float _rotationSpeed = 10f;
        
        private CardinalDirection _currentDirection;
        private Vector3 _initialMousePosition;
        private bool _isDragging;

        private void Start()
        {
            _currentDirection = CardinalDirection.North;
        }

        private void OnMouseDown()
        {
            _initialMousePosition = Input.mousePosition;
            _isDragging = true;
        }

        private void OnMouseDrag()
        {
            if (!_isDragging) return;

            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 mouseDelta = currentMousePosition - _initialMousePosition;

            float rotationAngle = mouseDelta.x * _rotationSpeed; // Adjust this to control sensitivity
            _pivotPoint.Rotate(Vector3.up, rotationAngle * Time.deltaTime);

            _initialMousePosition = currentMousePosition;
        }
        
        private void OnMouseUp()
        {
            _isDragging = false;
            RotateClockwise();
        }

        private void RotateClockwise()
        {
            _currentDirection = (CardinalDirection)(((int)_currentDirection + 1) % 4);
            
            foreach (DynamicNeighborTile endTile in _endTiles[_currentDirection])
                endTile.SetNeighbourTile();
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
            Tile.SetNeighbour(NeighbourDirection, Neighbour);

            if (Neighbour == null)
                GameCore.Instance.RefreshTilesNeighbours();
            else
                Neighbour.SetNeighbour(TileDirection, Tile);
        }
    }

    public enum CardinalDirection
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3
    }
}