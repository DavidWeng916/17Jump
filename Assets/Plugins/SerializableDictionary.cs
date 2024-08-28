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

        public Dictionary<TKey, TValue> Dict = null;

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            Dict = ToDictionary();
        }

        private Dictionary<TKey, TValue> ToDictionary()
        {
            Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();

            for (int i = 0; i < _list.Count; i++)
            {
                DictionaryData data = _list[i];

                if (!dict.TryAdd(data.Key, data.Value))
                {
                    Debug.LogError($"Duplicate Key:{data.Key}");
                }
            }

            return dict;
        }
    }
}