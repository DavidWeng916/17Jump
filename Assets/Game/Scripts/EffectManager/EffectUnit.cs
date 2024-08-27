using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Live17Game
{
    public abstract class EffectUnit : MonoBehaviour
    {
        [SerializeField]
        private EffectID _effectID;
        public EffectID EffectID => _effectID;

        public Action<EffectUnit> onComplete = null;

        public virtual void Init()
        {
            Reset();
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Reset()
        {
            onComplete = null;
        }

        public virtual void DestroySelf()
        {
            Destroy(gameObject);
        }

        public abstract void Play(Vector3 position, Quaternion rotation);

        public abstract void Stop();

        protected void OnComplete()
        {
            PropagateComplete(this);
        }

        protected void PropagateComplete(EffectUnit effectUnit)
        {
            onComplete(effectUnit);
        }
    }
}