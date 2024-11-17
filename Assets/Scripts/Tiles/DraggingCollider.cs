using System;
using UnityEngine;

public class DraggingCollider : MonoBehaviour
{
    public Action<Vector3> BeginClickEvent;
    public Action<Vector3> ClickEvent;
    public Action EndClickEvent;
    
    private bool _isDragging = false;
    
    private void OnMouseDown()
    {
        _isDragging = true;
        BeginClickEvent?.Invoke(Input.mousePosition);
    }

    private void OnMouseDrag()
    {
        if (_isDragging)
            ClickEvent?.Invoke(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        if(_isDragging)
            EndClickEvent?.Invoke();

        _isDragging = false;
    }
}