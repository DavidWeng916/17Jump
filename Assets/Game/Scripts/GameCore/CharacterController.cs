using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Live17Game
{
    public class CharacterController : MonoBehaviour
    {
        private const float JUMP_HEIGHT = 4f;

        private const float SCALE_Y_MIN = 0.4f;
        private const float SCALE_Y_MAX = 1f;

        private const float SCALE_XZ_MIN = 1f;
        private const float SCALE_XZ_MAX = 2f;

        private const float ANGLE_PER_CIRCLE = 360f;
        private const float ROLL_PER_DISTAMCE = 8f;

        [SerializeField]
        private Transform _characterScaleY = null;

        [SerializeField]
        private Transform _characterScaleXZ = null;

        [SerializeField]
        private Transform _characterContainer = null;

        [SerializeField]
        private Transform _trailRendererContainer = null;

        [SerializeField]
        private TrailRenderer _trailRenderer = null;

        private float _modelRadius = 0.5f;

        private CharacterUnit _characterUnit = null;

        public Vector3 LocalPosition => transform.localPosition;
        public Vector3 WorldPosition => transform.position;

        public Func<PlatformUnit> onRequestCurrentPlatformUnit = null;
        public Func<PlatformUnit> onRequestTargetPlatformUnit = null;
        public Action onSpawnAnimationComplete = null;
        public Action onJumpStart = null;
        public Action<Vector3> onJumpComplete = null;

        public void Init()
        {
            LoadCharacterUnit(1);
        }

        private void LoadCharacterUnit(uint id)
        {
            UnloadCharacterUnit();

            CharacterUnit characterUnitPrefab = Resources.Load<CharacterUnit>($"Prefab/Character/character-{id}");
            _characterUnit = Instantiate(characterUnitPrefab, Vector3.zero, Quaternion.identity, _characterContainer);

            AttachTrailRendererToCharacterUnit();
        }

        private void UnloadCharacterUnit()
        {
            RecycleTrailRenderer();

            if (_characterUnit != null)
            {
                _characterUnit.DestroySelf();
                _characterUnit = null;
            }
        }

        private void AttachTrailRendererToCharacterUnit()
        {
            if (_characterUnit == null)
            {
                return;
            }

            SetTrailRendererParent(_characterUnit.TrailContainer);
        }

        private void RecycleTrailRenderer()
        {
            SetTrailRendererParent(_trailRendererContainer);
        }

        private void SetTrailRendererParent(Transform parentTs)
        {
            _trailRenderer.transform.SetParent(parentTs);
            _trailRenderer.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        public void RefreshOriginPositionAndForward()
        {
            PlatformUnit currentPlatformUnit = GetCurrentPlatformUnit();
            transform.localPosition = currentPlatformUnit.PlatformLocalPoint + Vector3.up * 6f;

            RefreshForward();
        }

        public void PlaySpawnAnimation()
        {
            const float duration = 1f;

            PlatformUnit currentPlatformUnit = GetCurrentPlatformUnit();

            Tween fallTween = transform
                .DOMoveY(currentPlatformUnit.PlatformLocalPoint.y, duration)
                .SetEase(Ease.OutBounce);

            // Tween rotationTween = _characterGo.transform
            //     .DOLocalRotate(new Vector3(360, 0, 0), 1f, RotateMode.FastBeyond360)
            //     .SetRelative(true)
            //     .SetEase(Ease.OutQuint)
            //     ;

            DOTween.Sequence()
                .SetLink(gameObject)
                .Append(fallTween)
                // .Join(rotationTween)
                .OnComplete(OnSpawnAnimationComplete)
                ;
        }

        private void OnSpawnAnimationComplete()
        {
            onSpawnAnimationComplete();
        }

        public void RefreshForward()
        {
            PlatformUnit targetPlatformUnit = GetTargetPlatformUnit();

            if (targetPlatformUnit == null)
            {
                SetLocalRotation(Quaternion.AngleAxis(0f, Vector3.up));
                return;
            }

            PlatformUnit currentPlatformUnit = GetCurrentPlatformUnit();

            Vector2 from = Vector2.up;
            Vector2 currentPoint = new Vector2(currentPlatformUnit.LocalPosition.x, currentPlatformUnit.LocalPosition.z);
            Vector2 nextPoint = new Vector2(targetPlatformUnit.LocalPosition.x, targetPlatformUnit.LocalPosition.z);
            Vector2 direction = (nextPoint - currentPoint).normalized;
            float angle = Vector2.SignedAngle(from, direction);
            float angleY = -1f * angle;

            SetLocalRotation(Quaternion.AngleAxis(angleY, Vector3.up));
        }

        public void Jump(float accumulateProgress)
        {
            RestoreScale();

            Vector3 localPosition = LocalPosition;
            Vector3 targetLocalPosition = GetTargetLocalPosition(localPosition, accumulateProgress);

#if UNITY_EDITOR
            if (PlayerController.IS_CHEAT_ENABLE)
            {
                PlatformUnit targetPlatformUnit = GetTargetPlatformUnit();
                targetLocalPosition = targetPlatformUnit.PlatformLocalPoint;
            }
#endif
            PlayJumpAnimation(localPosition, targetLocalPosition);
            onJumpStart();
        }

        private Vector3 GetTargetLocalPosition(Vector3 localPosition, float accumulateProgress)
        {
            PlatformUnit targetPlatformUnit = GetTargetPlatformUnit();
            Vector3 nearestPoint = targetPlatformUnit.GetNearestPointFromCenter();
            Vector3 farestPoint = targetPlatformUnit.GetFarthestPointFromCenter();

            return GameLogicUtility.Interpolate(localPosition, nearestPoint, farestPoint, accumulateProgress);
        }

        private void PlayJumpAnimation(Vector3 startPoint, Vector3 endPoint)
        {
            float distance = MathUtility.DistanceXZ(startPoint, endPoint);
            int time = Mathf.CeilToInt(distance / ROLL_PER_DISTAMCE);
            float angle = ANGLE_PER_CIRCLE * time;

            // Debug.Log($"distance:{distance} time:{time} angle:{angle}");

            DOTween.To(setter: value =>
            {
                SetLocalPoaition(ParabolaMath.Parabola(startPoint, endPoint, JUMP_HEIGHT, value));
                SetCharacterLocalRotationX(angle * value);

            }, startValue: 0, endValue: 1, duration: 0.5f)
            .SetLink(gameObject)
            .SetEase(Ease.Linear)
            .OnComplete(OnJumpComplete);
        }

        private void OnJumpComplete()
        {
            onJumpComplete(transform.localPosition);
        }

        private float GetFallDownAngle()
        {
            PlatformUnit currentPlatformUnit = GetCurrentPlatformUnit();
            PlatformUnit targetPlatformUnit = GetTargetPlatformUnit();

            List<PlatformPoint> platformPointList = new List<PlatformPoint>
            {
                new PlatformPoint(LocalPosition, currentPlatformUnit.GetNearestPointFromCenter(), 90f),
                new PlatformPoint(LocalPosition, targetPlatformUnit.GetNearestPointFromCenter(), -90f),
                new PlatformPoint(LocalPosition, targetPlatformUnit.GetFarthestPointFromCenter(), 90f)
            };

            platformPointList.Sort((a, b) => a.Distance - b.Distance > 0 ? 1 : -1);
            PlatformPoint platformPoint = platformPointList[0];

            float distance = MathUtility.DistanceXZ(platformPoint.PlayerPoint, platformPoint.ReferencePoint);
            float angle = distance < _modelRadius ? platformPoint.FallDownAngle : 0f;

            return angle;
        }

        public void PlayFallDownAnimation(Action callback)
        {
            float duration = 0.25f;
            float angle = GetFallDownAngle();

            Tweener moveTween = transform.DOLocalMoveY(angle == 0f ? 0f : _modelRadius, duration);
            Tweener rotateTween = transform.DOLocalRotateQuaternion(transform.localRotation * Quaternion.AngleAxis(angle, Vector3.right), duration);

            DOTween.Sequence()
                .SetLink(gameObject)
                .Append(moveTween)
                .Join(rotateTween)
                .OnComplete(() => callback())
                ;
        }

        private void SetLocalPoaitionY(float y)
        {
            Vector3 localPosition = LocalPosition;
            localPosition.y = y;
            SetLocalPoaition(localPosition);
        }

        private void SetLocalPoaition(Vector3 localPosition)
        {
            transform.localPosition = localPosition;
        }

        private void SetLocalRotation(Quaternion localRotation)
        {
            transform.localRotation = localRotation;
        }

        public void UpdateScaleHeight(float accumulateProgress, float offsetY)
        {
            float scaleXZ = Mathf.Lerp(SCALE_XZ_MIN, SCALE_XZ_MAX, accumulateProgress);
            float scaleY = Mathf.Lerp(SCALE_Y_MAX, SCALE_Y_MIN, accumulateProgress);

            SetLocalScaleY(scaleY);
            SetLocalScaleXZ(scaleXZ);
            SetLocalPoaitionY(LocalPosition.y - offsetY);
        }

        private void RestoreScale()
        {
            SetLocalScaleY(1f);
            SetLocalScaleXZ(1f);
        }

        private void SetLocalScaleY(float scale)
        {
            _characterScaleY.localScale = new Vector3(1, scale, 1);
        }

        private void SetLocalScaleXZ(float scale)
        {
            _characterScaleXZ.localScale = new Vector3(scale, 1, scale);
        }

        private void SetCharacterLocalRotationX(float angleX)
        {
            _characterUnit.RotateXTs.localRotation = Quaternion.AngleAxis(angleX, Vector3.right);
        }

        private PlatformUnit GetCurrentPlatformUnit()
        {
            return onRequestCurrentPlatformUnit();
        }

        private PlatformUnit GetTargetPlatformUnit()
        {
            return onRequestTargetPlatformUnit();
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            Gizmos.color = Color.green;
            Gizmos.DrawRay(WorldPosition, Vector3.up);
        }
    }
}