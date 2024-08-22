using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Live17Game
{
    public abstract class ObjectPoolManagerBase : MonoBehaviour
    {
        public const int DEFAULT_CAPACITY = 10;
        public const int MAX_SIZE = 10000;
    }

    public abstract class ObjectPoolManagerBase<TPrefab, TContainer> : ObjectPoolManagerBase
        where TPrefab : ObjectPoolUnitBase
        where TContainer : Transform
    {
        [SerializeField]
        private TContainer _container = null;
        public TContainer Container => _container;

        [SerializeField]
        private TPrefab _prefab = null;

        private ObjectPool<TPrefab> _objectPool = null;

        public virtual void Init(int defaultCapacity = DEFAULT_CAPACITY, int maxSize = MAX_SIZE)
        {
            _objectPool = new ObjectPool<TPrefab>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, defaultCapacity, maxSize);
        }

        private TPrefab CreatePooledItem()
        {
            TPrefab unit = Instantiate<TPrefab>(_prefab, _container);
            unit.Init();
            return unit;
        }

        private void OnTakeFromPool(TPrefab unit)
        {
            unit.Show();
        }

        private void OnReturnedToPool(TPrefab unit)
        {
            unit.Hide();
            unit.Reset();
        }

        private void OnDestroyPoolObject(TPrefab unit)
        {
            unit.DestroySelf();
        }

        public virtual void ReleaseObjectPool()
        {
            _objectPool.Clear();
            _objectPool.Dispose();
        }

        protected virtual TPrefab Obtain()
        {
            return _objectPool.Get();
        }

        protected virtual void Release(TPrefab unit)
        {
            _objectPool.Release(unit);
        }
    }
}