using UnityEngine;

namespace Gameplay
{
    public class GameCore : MonoBehaviour
    {
        public static GameCore Instance;
    
        [field:SerializeField] public PlayerController Player {get; private set;}

        private int _currentColor;

        private void Awake()
        {
            if(Instance == null)
                Instance = this;
        }

        public void EndGame()
        {
            Debug.Log("You won");
        }
    }
}