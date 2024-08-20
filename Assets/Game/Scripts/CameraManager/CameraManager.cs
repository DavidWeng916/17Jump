using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Live17Game
{
    public class CameraManager : MonoBehaviour
    {
        private const float DISTANCE = 15f;

        [SerializeField]
        private Transform _cameraTs = null;

        // private float _followPointToCameraDistance = 0f;

        public void Init(Vector3 followPoint)
        {
            _cameraTs = transform;
            // _followPointToCameraDistance = Vector3.Distance(followPoint, _cameraTs.localPosition);
            // _followPointToCameraDistance = 15f;

            SetFollowTarget(followPoint, false);
        }

        public void SetFollowTarget(Vector3 followPoint, bool isAnimate)
        {
            Vector3 direction = -_cameraTs.forward;
            Vector3 finallPoint = followPoint + direction * DISTANCE;

            if (isAnimate)
            {
                _cameraTs.DOLocalMove(finallPoint, 0.5f).SetLink(gameObject);
            }
            else
            {
                _cameraTs.localPosition = finallPoint;
            }
        }

        void OnDrawGizmos()
        {
            Ray ray = new Ray(_cameraTs.localPosition, _cameraTs.forward);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(ray.origin, ray.direction * DISTANCE);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(ray.GetPoint(DISTANCE), 0.1f);
        }
    }
}