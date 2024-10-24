using System;
using DG.Tweening;
using UnityEngine;

[ExecuteInEditMode]
public class Tile : MonoBehaviour
{
    [SerializeField] private Transform _playerPosition;
    [SerializeField] private Collider _collider;
    [SerializeField] private float _neighbourRayLength = 1.2f;
    [SerializeField] private float _punchPositionForce;
    [SerializeField] private float _punchPositionDuration;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material[] _materials;

    private Tile[] _neighbors = new Tile[4];
    public Transform PlayerPosition => _playerPosition;
    public Tile[] Neighbors => _neighbors;

    private void Start()
    {
        FindNeighbours();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            FindNeighbours();
#endif
    }

    public void RefreshMaterial(bool isCurrent)
    {
        _meshRenderer.sharedMaterial = isCurrent ? _materials[1] : _materials[0];
    }

    private void FindNeighbours()
    {
        Vector3[] directions = GetDirections();
        for (int index = 0; index < directions.Length; index++)
        {
            bool detectBlock = TryFindNeighbour(directions[index], out RaycastHit hit);

            if (detectBlock)
                Neighbors[index] = hit.collider.GetComponent<Tile>();
        }
    }

    private bool TryFindNeighbour(Vector3 direction, out RaycastHit hit)
    {
        float maxDistance = _collider.bounds.extents.y * _neighbourRayLength;
        bool detectBlock = Physics.Raycast(transform.position, direction, out hit, maxDistance);
        Debug.DrawLine(transform.position, transform.position + direction * maxDistance, detectBlock ? Color.green : Color.red);

        return detectBlock;
    }

    private Vector3[] GetDirections()
    {
        Vector3[] directions =
        {
            transform.forward,
            transform.right,
            -transform.right,
            -transform.forward
        };

        return directions;
    }

    private Tween _punchPositionTween;

    private void OnMouseDown()
    {
        GameCore.Instance.Player.Move(this);

        _punchPositionTween?.Complete();
        _punchPositionTween = transform.DOPunchPosition(Vector3.up * -_punchPositionForce, _punchPositionDuration, 0, 0);
    }
}