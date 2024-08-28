using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Live17Game
{
    public enum EffectID
    {
        None = 0,
        LightCircle = 1,
    }

    public class EffectManager : MonoBehaviour
    {
        public static EffectManager Instance = null;

        public const int DEFAULT_CAPACITY = 10;
        public const int MAX_SIZE = 10000;

        [SerializeField]
        private Transform _container = null;
        public Transform Container => _container;

        [SerializeField]
        private SerializableDictionary<EffectID, EffectUnit> _effectPrefabMap = new SerializableDictionary<EffectID, EffectUnit>();

        private Dictionary<EffectID, ObjectPool<EffectUnit>> _objectPoolMap = new Dictionary<EffectID, ObjectPool<EffectUnit>>();

        public void Init()
        {
            Instance = this;
        }

        private EffectUnit CreatePooledItem(EffectID effectID)
        {
            if (!_effectPrefabMap.TryGetValue(effectID, out EffectUnit effectUnit))
            {
                throw new System.Exception($"Error, could't find effectID:{effectID}");
            }

            EffectUnit unit = Instantiate<EffectUnit>(effectUnit, _container);
            unit.Init();
            return unit;
        }

        private void OnTakeFromPool(EffectUnit unit)
        {
            unit.Show();
        }

        private void OnReturnedToPool(EffectUnit unit)
        {
            unit.Hide();
            unit.Reset();
        }

        private void OnDestroyPoolObject(EffectUnit unit)
        {
            unit.DestroySelf();
        }

        public void ReleaseObjectPool()
        {
            foreach (var item in _objectPoolMap)
            {
                item.Value.Clear();
                item.Value.Dispose();
            }
        }

        protected EffectUnit Obtain(EffectID effectID)
        {
            if (!_objectPoolMap.TryGetValue(effectID, out ObjectPool<EffectUnit> objectPool))
            {
                objectPool = new ObjectPool<EffectUnit>(() => CreatePooledItem(effectID), OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, DEFAULT_CAPACITY, MAX_SIZE);
                _objectPoolMap.TryAdd(effectID, objectPool);
            }

            return objectPool.Get();
        }

        protected void Release(EffectUnit unit)
        {
            if (_objectPoolMap.TryGetValue(unit.EffectID, out ObjectPool<EffectUnit> objectPool))
            {
                objectPool.Release(unit);
            }
        }

        void OnDestroy()
        {
            Instance = null;
        }

        public void Play(EffectID effectID, Vector3 position, Quaternion rotation)
        {
            EffectUnit effectUnit = Obtain(effectID);
            effectUnit.onComplete = OnRecycle;
            effectUnit.Play(position, rotation);
        }

        private void OnRecycle(EffectUnit effectUnit)
        {
            Release(effectUnit);
        }
    }
}