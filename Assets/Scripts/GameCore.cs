using System;
using UnityEngine;

public class GameCore : MonoBehaviour
{
    public static GameCore Instance;
    
    [SerializeField] private Tile[] _tileWithDynamicNeighbours;
    [field:SerializeField] public PlayerController Player {get; private set;}

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
}