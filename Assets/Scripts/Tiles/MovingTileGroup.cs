using System;
using UnityEngine;

namespace Tiles
{
    public class MovingTileGroup : TileGroup
    {
        [SerializeField] private Vector3 _movingDirection;
        
        private void Start()
        {
            Initialize();
        }

        protected override void OnToolBeginClick(Vector3 mousePosition)
        {
            _isDragging = true;
            _initialMousePosition = mousePosition;
        }

        protected override void OnToolClick(Vector3 mousePosition)
        {
            if (!_isDragging) return;
            
        }

        protected override void OnToolEndClick()
        {
            _isDragging = false;
        }

        private void OnDestroy()
        {
            Destroy();
        }
    }
}