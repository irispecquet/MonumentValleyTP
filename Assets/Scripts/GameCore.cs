using System;
using UnityEngine;

public class GameCore : MonoBehaviour
{
    public static GameCore Instance;
    
    [SerializeField] private Tile[] _tileWithDynamicNeighbours;
    [SerializeField] private Tile[] _tilesToReverse;
    [SerializeField] private Material[] _tileMaterials;
    
    [field:SerializeField] public PlayerController Player {get; private set;}

    private int _currentColor;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    public void RefreshTilesNeighbours()
    {
        foreach (Tile tile in _tileWithDynamicNeighbours)
            tile.FindNeighbours();
    }

    public void ReverseColor()
    {
        int newColor = _currentColor == 0 ? 1 : 0;
            
        foreach (Tile tile in _tilesToReverse)
        {
            tile.ChangeMaterial(_tileMaterials[newColor]);
        }

        _currentColor = newColor;
    }
}