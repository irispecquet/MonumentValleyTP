using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class Fade : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private float _fadeSpeed = 1f;

        private Tween _fadeTween;

        private void FadeImage(Action action, int alpha)
        {
            _fadeTween?.Kill();
            _fadeTween = _image.DOFade(alpha, _fadeSpeed).OnComplete(() =>
            {
                _image.raycastTarget = alpha == 1;
                action?.Invoke();
            });
        }
        
        public void FadeOut(Action action = null) => FadeImage(action, 0);
        public void FadeIn(Action action = null) => FadeImage(action, 1);
    }
}