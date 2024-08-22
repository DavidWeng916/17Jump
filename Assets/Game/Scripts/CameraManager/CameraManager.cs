using UnityEngine;
using DG.Tweening;

namespace Live17Game
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField]
        private Transform _cameraTs = null;

        private Tween _tween = null;

        public void Init(Vector3 followPoint)
        {
            _cameraTs = transform;

            SetFollowTarget(followPoint, false);
        }

        public void SetFollowTarget(Vector3 followPoint, bool isAnimate)
        {
            KillTween();

            Vector3 direction = -_cameraTs.forward;
            Vector3 finallPoint = followPoint + direction * DataModel.CAMERA_DISTANCE;

            if (isAnimate)
            {
                _tween = _cameraTs.DOLocalMove(finallPoint, DataModel.CAMERA_MOVE_DURATION).SetLink(gameObject);
            }
            else
            {
                _cameraTs.localPosition = finallPoint;
            }
        }

        private void KillTween()
        {
            if (_tween != null)
            {
                _tween.Kill(true);
                _tween = null;
            }
        }

        private Ray ray = new Ray();
        void OnDrawGizmos()
        {
            ray.origin = _cameraTs.localPosition;
            ray.direction = _cameraTs.forward;

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(ray.origin, ray.direction * DataModel.CAMERA_DISTANCE);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(ray.GetPoint(DataModel.CAMERA_DISTANCE), 0.1f);
        }
    }
}