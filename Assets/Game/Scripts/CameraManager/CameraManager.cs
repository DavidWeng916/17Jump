using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Live17Game
{
    public class CameraManager : MonoBehaviour
    {
        private Transform _cameraTs = null;

        private float _followPointToCameraDistance = 0f;

        public void Init(Vector3 followPoint)
        {
            _cameraTs = transform;
            _followPointToCameraDistance = Vector3.Distance(followPoint, _cameraTs.localPosition);

            SetFollowTarget(followPoint, false);
        }

        public void SetFollowTarget(Vector3 followPoint, bool isAnimate)
        {
            Vector3 direction = -_cameraTs.forward;
            Vector3 finallPoint = followPoint + direction * _followPointToCameraDistance;

            if (isAnimate)
            {
                _cameraTs.DOLocalMove(finallPoint, 0.5f).SetLink(gameObject);
            }
            else
            {
                _cameraTs.localPosition = finallPoint;
            }
        }
    }
}