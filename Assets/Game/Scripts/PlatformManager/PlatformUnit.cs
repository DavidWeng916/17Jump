using UnityEngine;
using DG.Tweening;

namespace Live17Game
{
    public class PlatformUnit : ObjectPoolUnitBase
    {
        [SerializeField]
        private GameObject _modelGo = null;

        public Vector3 LocalPosition => transform.localPosition;
        public Quaternion LocalRotation => transform.localRotation;

        public float Radius { get; private set; } = 0f;
        public Vector3 SizeVec { get; private set; }
        public Vector3 HeightVec { get; private set; }
        public float HalfHeight { get; private set; } = 0f;
        public Vector3 PlatformLocalPoint => LocalPosition + HeightVec;

        public void SetData(uint size)
        {
            SizeVec = new Vector3(size, DataModel.DEFAULT_PLATFORM_HEIGHT, size);
            Radius = SizeVec.x * 0.5f;
            HeightVec = new Vector3(0, SizeVec.y, 0);
            HalfHeight = SizeVec.y * 0.5f;

            transform.localScale = SizeVec;
        }

        public Vector3 GetNearestPoint()
        {
            float z = SizeVec.z * 0.5f;
            return PlatformLocalPoint + LocalRotation * new Vector3(0, 0, z);
        }
        public Vector3 GetFarthestPoint()
        {
            float z = SizeVec.z * -0.5f;
            return PlatformLocalPoint + LocalRotation * new Vector3(0, 0, z);
        }

        public void SetLocalPositionAndRotation(Vector3 localPosition, Quaternion localRotation)
        {
            // localPosition.y = 0f;
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
    }
}