using System;
using LuniLiiiib.UnityUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private SceneField _gameScene;
        [SerializeField] private Fade _fade;

        private void Start()
        {
            _fade.FadeOut();
        }

        public void GoToMainScene()
        {
            _fade.FadeIn(() => SceneManager.LoadScene(_gameScene));
        }
    }
}