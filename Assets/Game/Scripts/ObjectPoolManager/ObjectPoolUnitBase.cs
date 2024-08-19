using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Live17Game
{
    public abstract class ObjectPoolUnitBase : MonoBehaviour
    {
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

        }

        public virtual void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}