using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tiles
{
    public abstract class TileGroup : SerializedMonoBehaviour
    {
        [SerializeField] protected DraggingCollider _draggingCollider;
        [SerializeField] protected float _speed;
        
        protected List<Tile> _tiles = new();
        protected Vector3 _initialMousePosition;
        protected bool _isDragging;

        protected void Initialize()
        {
            if (_draggingCollider != null)
            {
                _draggingCollider.BeginClickEvent += OnToolBeginClick;
                _draggingCollider.ClickEvent += OnToolClick;
                _draggingCollider.EndClickEvent += OnToolEndClick;
            }
        }

        protected void Destroy()
        {
            if (_draggingCollider != null)
            {
                _draggingCollider.BeginClickEvent -= OnToolBeginClick;
                _draggingCollider.ClickEvent -= OnToolClick;
                _draggingCollider.EndClickEvent -= OnToolEndClick;
            }
        }
        
        protected abstract void OnToolBeginClick(Vector3 mousePosition);

        protected abstract void OnToolClick(Vector3 mousePosition);

        protected abstract void OnToolEndClick();
    }
}