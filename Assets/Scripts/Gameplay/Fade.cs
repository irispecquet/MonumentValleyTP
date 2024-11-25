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

        private void FadeImage(Action onCompleteAction, int alpha)
        {
            _fadeTween?.Kill();
            _fadeTween = _image.DOFade(alpha, _fadeSpeed).OnComplete(() =>
            {
                _image.raycastTarget = alpha == 1;
                onCompleteAction?.Invoke();
            });
        }
        
        public void FadeOut(Action onCompleteAction = null) => FadeImage(onCompleteAction, 0);
        public void FadeIn(Action onCompleteAction = null) => FadeImage(onCompleteAction, 1);
    }
}