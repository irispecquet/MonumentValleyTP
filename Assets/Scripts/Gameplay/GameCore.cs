using JetBrains.Annotations;
using LuniLiiiib.UnityUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
{
    public class GameCore : MonoBehaviour
    {
        public static GameCore Instance;

        [SerializeField, CanBeNull] private SceneField _nextLevelScene;
        [SerializeField, CanBeNull] private GameObject _endGamePanel;
        [SerializeField] private Fade _fade;
    
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
        }

        public void GoToNextLevel()
        {
            _fade.FadeIn(() =>
            {
                if(string.IsNullOrEmpty(_nextLevelScene.SceneName))
                    _endGamePanel.SetActive(true);
                else
                    SceneManager.LoadScene(_nextLevelScene);
            });
        }
    }
}