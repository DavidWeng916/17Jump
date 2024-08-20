using UnityEngine;
using DG.Tweening;

namespace Live17Game
{
    public class PlatformUnit : ObjectPoolUnitBase
    {
        [SerializeField]
        private GameObject _modelGo = null;

        private PlatformUnit _oppositePlatformUnit = null;

        public Vector3 SizeVec { get; private set; }
        public Vector3 HeightVec { get; private set; }
        public float Radius { get; private set; } = 0f;
        public float HalfHeight { get; private set; } = 0f;

        public Vector3 LocalPosition => transform.localPosition;
        public Quaternion LocalRotation => transform.localRotation;
        public Vector3 PlatformLocalPoint => LocalPosition + HeightVec;

        public void SetData(PlatformData platformData)
        {
            SizeVec = new Vector3(platformData.Size, DataModel.DEFAULT_PLATFORM_HEIGHT, platformData.Size);
            HeightVec = new Vector3(0, SizeVec.y, 0);
            Radius = SizeVec.x * 0.5f;
            HalfHeight = SizeVec.y * 0.5f;

            transform.localScale = SizeVec;

            SetLocalPositionAndRotation(platformData.LocalPosition, Quaternion.AngleAxis(platformData.AngleY, Vector3.up));
        }

        public void SetOppositePlatformUnit(PlatformUnit oppositePlatformUnit)
        {
            _oppositePlatformUnit = oppositePlatformUnit;
        }

        public Vector3 GetNearestPointFromCenter()
        {
            return GetNearestOrFarthestPoint(1);
        }

        public Vector3 GetFarthestPointFromCenter()
        {
            return GetNearestOrFarthestPoint(-1);
        }

        private Vector3 GetNearestOrFarthestPoint(int sign)
        {
            Vector3 direction = MathUtility.DirectionXZ(LocalPosition, _oppositePlatformUnit.LocalPosition);
            // return PlatformLocalPoint + sign * direction * Radius;

            Vector3 closestAxis = MathUtility.GetClosestDirection(transform, direction);
            return PlatformLocalPoint + sign * closestAxis * Radius;
        }

        public void SetLocalPositionAndRotation(Vector3 localPosition, Quaternion localRotation)
        {
            transform.SetLocalPositionAndRotation(localPosition, localRotation);
        }

        public float UpdateScaleHeight(float accumulateProgress)
        {
            Vector3 localScale = transform.localScale;
            float lastHeight = localScale.y;
            localScale.y = Mathf.Lerp(SizeVec.y, HalfHeight, accumulateProgress);
            transform.localScale = localScale;

            return lastHeight - localScale.y;
        }

        public void RestoreScaleHeight()
        {
            transform
            .DOScaleY(SizeVec.y, 0.3f)
            .SetLink(gameObject)
            .SetEase(Ease.OutBounce);
        }

        public void PlaySpawnAnimation()
        {
            Vector3 originPoint = _modelGo.transform.localPosition;
            Vector3 fromPoint = new Vector3(originPoint.x, 5f, originPoint.z);

            _modelGo.transform
                .DOLocalMoveY(0, 0.5f)
                .From(fromPoint, true, true)
                .SetLink(gameObject)
                .SetEase(Ease.OutBounce)
                ;
        }

        public void ResetData()
        {
            _oppositePlatformUnit = null;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(GetNearestPointFromCenter(), Vector3.up * 3f);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(GetFarthestPointFromCenter(), Vector3.up * 3f);
        }
    }
}