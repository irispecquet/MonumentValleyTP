using JetBrains.Annotations;
using UnityEngine;

namespace Gameplay
{
    public class GameCore : MonoBehaviour
    {
        public static GameCore Instance;

        [SerializeField] private LevelManager _levelManager;
        [SerializeField, CanBeNull] private GameObject _endGamePanel;
        [SerializeField] private Fade _fade;
        [SerializeField] private int _startingLevelIndex;
    
        [field:SerializeField] public PlayerController Player {get; private set;}

        private int _currentColor;

        private void Awake()
        {
            if(Instance == null)
                Instance = this;
        }

        private void Start()
        {
            _fade.FadeOut();
            
            _levelManager.Init(_startingLevelIndex, Player);
        }

        public void GoToNextLevel()
        {
            _fade.FadeIn(() =>
            {
                if (!_levelManager.TryLaunchNextLevel())
                    _endGamePanel?.SetActive(true);
                else
                    _fade.FadeOut();
            });
        }
    }
}