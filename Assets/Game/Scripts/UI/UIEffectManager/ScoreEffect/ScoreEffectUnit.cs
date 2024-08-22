using System;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace Live17Game
{
    public class ScoreEffectUnit : ObjectPoolUnitBase
    {
        private const float MOVE_POINT_Y_MAX = 100f;

        [SerializeField]
        private TextMeshProUGUI _scoreLabel = null;

        private RectTransform _rt = null;
        private Sequence _sequenceTween = null;
        private CoordConvertData _coordConvertData;

        public Action<ScoreEffectUnit> onAnimationComplete = null;

        public override void Init()
        {
            base.Init();

            _rt = GetComponent<RectTransform>();
        }

        public override void Reset()
        {
            base.Reset();

            _scoreLabel.SetText(string.Empty);
            _scoreLabel.alpha = 1f;

            onAnimationComplete = null;
        }

        public void Show(CoordConvertData coordConvertData, uint score)
        {
            _coordConvertData = coordConvertData;
            _scoreLabel.SetText($"+{score}");

            UpdateLocalPosition(0f);

            base.Show();

            PlayTween();
        }

        private void PlayTween(float duration = 1f)
        {
            KillTween();

            float offsetY = 0f;
            Tween offsetYtween = DOTween.To(() => offsetY, x => offsetY = x, MOVE_POINT_Y_MAX, duration)
                .OnUpdate(() =>
                {
                    UpdateLocalPosition(offsetY);
                });

            Tween alphaTween = _scoreLabel
                .DOFade(0f, duration)
                .SetEase(Ease.InCirc);

            _sequenceTween = DOTween.Sequence()
                .SetLink(gameObject)
                .Append(offsetYtween)
                .Join(alphaTween)
                .OnComplete(OnAnimationComplete);
        }

        private void KillTween()
        {
            if (_sequenceTween != null)
            {
                _sequenceTween.Kill(true);
                _sequenceTween = null;
            }
        }

        private void UpdateLocalPosition(float offsetY)
        {
            _rt.anchoredPosition = new Vector2(0, offsetY) + CoordinateUtility.ConvertWorldPointToCanvas(_coordConvertData);
        }

        private void OnAnimationComplete()
        {
            onAnimationComplete?.Invoke(this);
        }
    }
}