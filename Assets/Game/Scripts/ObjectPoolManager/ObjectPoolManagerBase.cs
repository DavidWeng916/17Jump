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

    public abstract class ObjectPoolManagerBase<T> : ObjectPoolManagerBase
        where T : ObjectPoolUnitBase
    {
        [SerializeField]
        private Transform _container = null;

        [SerializeField]
        private T _prefab = null;

        private ObjectPool<T> _objectPool = null;

        public Transform Container => _container;

        public virtual void Init(int defaultCapacity = DEFAULT_CAPACITY, int maxSize = MAX_SIZE)
        {
            _objectPool = new ObjectPool<T>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, defaultCapacity, maxSize);
        }

        private T CreatePooledItem()
        {
            T unit = Instantiate<T>(_prefab, _container);
            unit.Init();
            return unit;
        }

        private void OnTakeFromPool(T unit)
        {
            unit.Show();
        }

        private void OnReturnedToPool(T unit)
        {
            unit.Hide();
            unit.Reset();
        }

        private void OnDestroyPoolObject(T unit)
        {
            unit.DestroySelf();
        }

        public virtual void ReleaseObjectPool()
        {
            _objectPool.Clear();
            _objectPool.Dispose();
        }

        protected virtual T Obtain()
        {
            return _objectPool.Get();
        }

        protected virtual void Release(T unit)
        {
            _objectPool.Release(unit);
        }
    }
}