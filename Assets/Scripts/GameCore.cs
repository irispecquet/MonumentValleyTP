using System;
using UnityEngine;

public class GameCore : MonoBehaviour
{
    public static GameCore Instance;
    
    [field:SerializeField] public PlayerController Player {get; private set;}

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }
}