using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Live17Game
{
    public class LightCircleEffectUnit : EffectUnit
    {
        private float _duration = 1f;
        private float _endSizeXZ = 0.8f;

        private Transform _ts = null;
        private Color _originColor = Color.white;
        private Material _mat = null;

        private Sequence _sequence = null;

        public override void Init()
        {
            _ts = transform;
            _mat = GetComponent<MeshRenderer>().material;
            _originColor = _mat.color;

            base.Init();
        }

        public override void Reset()
        {
            base.Reset();

            _ts.localScale = Vector3.zero;
            _mat.color = _originColor;
        }

        public override void Play(Vector3 position, Quaternion rotation)
        {
            _ts.SetPositionAndRotation(position, rotation);

            Tween scaleTween = _ts.DOScale(new Vector3(_endSizeXZ, 1f, _endSizeXZ), _duration).SetEase(Ease.OutQuad);
            Tween alphaTween = _mat.DOFade(0f, _duration).SetEase(Ease.InQuad);

            _sequence = DOTween.Sequence()
                .SetLink(gameObject)
                .Append(scaleTween)
                .Join(alphaTween)
                .OnComplete(OnComplete);
        }

        public override void Stop()
        {
            KillTween();
        }

        private void KillTween()
        {
            if (_sequence != null)
            {
                _sequence.Kill(true);
                _sequence = null;
            }
        }
    }
}