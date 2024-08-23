using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Live17Game
{
    public class CharacterUnit : MonoBehaviour
    {
        private const float JUMP_HEIGHT = 4f;

        private const float PLAYER_SCALE_Y_MIN = 0.4f;
        private const float PLAYER_SCALE_Y_MAX = 1f;

        private const float PLAYER_SCALE_XZ_MIN = 1f;
        private const float PLAYER_SCALE_XZ_MAX = 2f;

        [SerializeField]
        private Transform _characterContainer = null;

        private float _modelRadius = 0.5f;

        private GameObject _characterGo = null;

        public Vector3 LocalPosition => transform.localPosition;
        public Vector3 WorldPosition => transform.position;

        public Func<PlatformUnit> onRequestCurrentPlatformUnit = null;
        public Func<PlatformUnit> onRequestTargetPlatformUnit = null;
        public Action onSpawnAnimationComplete = null;
        public Action onJumpStart = null;
        public Action<Vector3> onJumpComplete = null;

        public void Init()
        {
            LoadCharacter(1);
        }

        private void LoadCharacter(uint id)
        {
            UnloadCharacter();

            GameObject prefab = Resources.Load<GameObject>($"Prefab/Character/character-{id}");
            _characterGo = Instantiate<GameObject>(prefab, _characterContainer);
        }

        private void UnloadCharacter()
        {
            if (_characterGo != null)
            {
                Destroy(_characterGo);
                _characterGo = null;
            }
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
            RestoreScaleHeight();

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

            return Interpolate(localPosition, nearestPoint, farestPoint, accumulateProgress);
        }

        public Vector3 Interpolate(Vector3 currentPoint, Vector3 nearestPoint, Vector3 farestPoint, float accumulateProgress)
        {
            float jumpLength = accumulateProgress * DataModel.JUMP_LENGTH;
            float distanceP0ToP1 = MathUtility.DistanceXZ(currentPoint, nearestPoint);

            if (jumpLength < distanceP0ToP1)
            {
                return MathUtility.GetLandPoint(currentPoint, nearestPoint, jumpLength);
            }

            return MathUtility.GetLandPoint(nearestPoint, farestPoint, jumpLength - distanceP0ToP1);
        }

        private void PlayJumpAnimation(Vector3 startPoint, Vector3 endPoint)
        {
            DOTween.To(setter: value =>
            {
                SetLocalPoaition(ParabolaMath.Parabola(startPoint, endPoint, JUMP_HEIGHT, value));
                SetCharacterLocalRotationX(360f * value);

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
            float scaleXZ = Mathf.Lerp(PLAYER_SCALE_XZ_MIN, PLAYER_SCALE_XZ_MAX, accumulateProgress);
            float scaleY = Mathf.Lerp(PLAYER_SCALE_Y_MAX, PLAYER_SCALE_Y_MIN, accumulateProgress);

            SetLocalScale(new Vector3(scaleXZ, scaleY, scaleXZ));
            SetLocalPoaitionY(LocalPosition.y - offsetY);
        }

        private void RestoreScaleHeight()
        {
            SetLocalScale(Vector3.one);
        }

        private void SetLocalScale(Vector3 localScale)
        {
            transform.localScale = localScale;
        }

        private void SetCharacterLocalRotationX(float angleX)
        {
            _characterGo.transform.localRotation = Quaternion.AngleAxis(angleX, Vector3.right);
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