using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace Live17Game
{
    public class PlayerController : MonoBehaviour
    {
        private const float JUMP_HEIGHT = 4f;
        private const float PLAYER_SCALE_Y_MIN = 0.4f;
        private const float PLAYER_SCALE_Y_MAX = 1f;

        private const float PLAYER_SCALE_XZ_MIN = 1f;
        private const float PLAYER_SCALE_XZ_MAX = 2f;

        [SerializeField]
        private Transform _character = null;

        private bool _isCanJump = false;
        private float _pressTime = 0f;
        private float _accumulateEnergySpeed = 2f;
        private Vector3 LocalPosition => transform.localPosition;

        public Func<PlatformUnit> onRequestCurrentPlatformUnit = null;
        public Func<PlatformUnit> onRequestTargetPlatformUnit = null;
        public Action onJumpStart = null;
        public Action<Vector3> onJumpComplete = null;
        public Action<float> onAccumulateEnergy = null;

        // public float jumpLength = 2.5f;

        public void Init()
        {
            SetControl(false);
        }

        void Update()
        {
            if (!_isCanJump)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                ResetEnergy();
                RefreshForward();
            }

            if (Input.GetMouseButton(0))
            {
                AccumulateEnergy();
            }

            if (Input.GetMouseButtonUp(0))
            {
                RestoreScaleHeight();
                Jump();
                SetControl(false);
            }

            // if (Input.GetKeyDown(KeyCode.Space)) Jump();
        }

        private void ResetEnergy()
        {
            _pressTime = 0;
        }

        private void AccumulateEnergy()
        {
            _pressTime = Mathf.Min(_pressTime + Time.deltaTime * _accumulateEnergySpeed, DataModel.PRESS_TME_MAX);
            float accumulateProgress = _pressTime / DataModel.PRESS_TME_MAX;
            onAccumulateEnergy(accumulateProgress);
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

        private void Jump()
        {
            Vector3 localPosition = LocalPosition;

            PlatformUnit targetPlatformUnit = GetTargetPlatformUnit();
            Vector3 nearestPoint = targetPlatformUnit.GetNearestPoint();
            Vector3 farestPoint = targetPlatformUnit.GetFarthestPoint();

            float jumpLength = _pressTime * DataModel.JUMP_LENGTH;
            float distanceXZ = MathUtility.GetXZDistance(localPosition, nearestPoint);

            Vector3 targetLocalPosition;

            if (jumpLength < distanceXZ)
            {
                targetLocalPosition = MathUtility.GetLandPoint(localPosition, nearestPoint, jumpLength);
            }
            else
            {
                targetLocalPosition = MathUtility.GetLandPoint(nearestPoint, farestPoint, jumpLength - distanceXZ);
            }

            PlayJumpAnimation(localPosition, targetLocalPosition);
            onJumpStart();
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

        public void RefreshOriginPositionAndForward()
        {
            PlatformUnit platformUnit = GetCurrentPlatformUnit();
            transform.localPosition = platformUnit.PlatformLocalPoint;
            RefreshForward();
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

        private PlatformUnit GetCurrentPlatformUnit()
        {
            return onRequestCurrentPlatformUnit();
        }

        private PlatformUnit GetTargetPlatformUnit()
        {
            return onRequestTargetPlatformUnit();
        }

        public void SetControl(bool isCanJump)
        {
            _isCanJump = isCanJump;
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

        private void SetLocalScale(Vector3 localScale)
        {
            transform.localScale = localScale;
        }

        private void SetCharacterLocalRotationX(float angleX)
        {
            _character.localRotation = Quaternion.AngleAxis(angleX, Vector3.right);
        }
    }
}