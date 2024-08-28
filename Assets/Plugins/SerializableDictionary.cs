using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Live17Game
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [Serializable]
        public class DictionaryData
        {
            public TKey Key;
            public TValue Value;
        }

        [SerializeField]
        private List<DictionaryData> _list = new List<DictionaryData>();

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            ToDictionary();
        }

        private void ToDictionary()
        {
            for (int i = 0; i < _list.Count; i++)
            {
                DictionaryData data = _list[i];

                if (!TryAdd(data.Key, data.Value))
                {
                    Debug.LogError($"Duplicate Key:{data.Key}");
                }
            }
        }
    }
}