using UnityEngine;
using DG.Tweening;

namespace Live17Game
{
    public class PlatformUnit : ObjectPoolUnitBase
    {
        private static readonly Vector3 DEFAULT_IDLE_POINT = new Vector3(-10f, 0, 0);
        private static readonly Quaternion DEFAULT_IDLE_QUAT = Quaternion.identity;

        [SerializeField]
        private Transform _platformScaleYTs = null;

        [SerializeField]
        private Transform _platformScaleXZTs = null;

        [SerializeField]
        private GameObject _centerTipGo = null;

        private PlatformUnit _oppositePlatformUnit = null;

        public Vector3 SizeVec { get; private set; }
        public Vector3 HeightVec { get; private set; }
        public float Radius { get; private set; } = 0f;
        public float HalfHeight { get; private set; } = 0f;

        public Vector3 LocalPosition
        {
            get
            {
                Vector3 localPosition = transform.localPosition;
                localPosition.y = 0;
                return localPosition;
            }
        }
        public Quaternion LocalRotation => transform.localRotation;
        public Vector3 PlatformLocalPoint => LocalPosition + HeightVec;

        public override void Init()
        {
            base.Init();

            InitCenterTip();
        }

        private void InitCenterTip()
        {
            Vector3 localScale = _centerTipGo.transform.localScale;
            localScale.x = DataModel.PERFECT_DIAMETER;
            localScale.z = DataModel.PERFECT_DIAMETER;

            _centerTipGo.transform.localScale = localScale;
        }

        public override void Reset()
        {
            base.Reset();

            SetLocalPositionAndRotation(DEFAULT_IDLE_POINT, DEFAULT_IDLE_QUAT);
            SetDisplayCenterTip(false);

            _oppositePlatformUnit = null;
        }

        public void SetData(PlatformData platformData)
        {
            SizeVec = new Vector3(platformData.Size, DataModel.DEFAULT_PLATFORM_HEIGHT, platformData.Size);
            HeightVec = new Vector3(0, SizeVec.y, 0);
            Radius = SizeVec.x * 0.5f;
            HalfHeight = SizeVec.y * 0.5f;

            _platformScaleYTs.localScale = new Vector3(1, SizeVec.y, 1);
            _platformScaleXZTs.localScale = new Vector3(SizeVec.x, 1, SizeVec.z);

            SetLocalPositionAndRotation(platformData.LocalPosition, Quaternion.AngleAxis(platformData.AngleY, Vector3.up));
        }

        public void SetDisplayCenterTip(bool isDisplay)
        {
            _centerTipGo.SetActive(isDisplay);
        }

        public void SetOppositePlatformUnit(PlatformUnit oppositePlatformUnit)
        {
            _oppositePlatformUnit = oppositePlatformUnit;
        }

        public void SetLocalPositionAndRotation(Vector3 localPosition, Quaternion localRotation)
        {
            transform.SetLocalPositionAndRotation(localPosition, localRotation);
        }

        public float UpdateScaleHeight(float accumulateProgress)
        {
            Vector3 localScale = _platformScaleYTs.localScale;
            float lastHeight = localScale.y;
            localScale.y = Mathf.Lerp(SizeVec.y, HalfHeight, accumulateProgress);
            _platformScaleYTs.localScale = localScale;

            return lastHeight - localScale.y;
        }

        public void RestoreScaleHeight()
        {
            _platformScaleYTs
                .DOScaleY(SizeVec.y, 0.3f)
                .SetLink(gameObject)
                .SetEase(Ease.OutBounce);
        }

        public void PlaySpawnAnimation()
        {
            Vector3 originPoint = LocalPosition;
            Vector3 fromPoint = new Vector3(originPoint.x, 5f, originPoint.z);

            transform
                .DOLocalMoveY(0, 0.5f)
                .From(fromPoint, true, true)
                .SetLink(gameObject)
                .SetEase(Ease.OutBounce)
                ;
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

        void OnDrawGizmos()
        {
            if (!Application.isPlaying || _oppositePlatformUnit == null)
            {
                return;
            }

            // Gizmos.color = Color.blue;
            // Gizmos.DrawRay(GetNearestPointFromCenter(), Vector3.up * 3f);
            // Gizmos.color = Color.red;
            // Gizmos.DrawRay(GetFarthestPointFromCenter(), Vector3.up * 3f);

            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawSphere(PlatformLocalPoint, DataModel.PERFECT_RADIUS);
        }
    }
}