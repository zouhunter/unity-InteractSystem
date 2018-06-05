using UnityEngine;
using System;
namespace WorldActionSystem
{

    [System.Serializable]
    public class RunTimePrefabItem : IComparable<RunTimePrefabItem>
    {
#if UNITY_EDITOR
        public int instanceID;
#endif
        private string _id; 
        public string ID
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    _id = CalcuteID(prefab);
                }
                return _id;
            }
        }
        public GameObject prefab;//ISupportElement
        public static string CalcuteID(GameObject prefab)
        {
            string _id = null;
            var name = prefab == null ? "Null" : prefab.name;
            _id = name;
            return _id;
        }
        public int CompareTo(RunTimePrefabItem other)
        {
            if (prefab == null || other.prefab == null) return 0;
            return string.Compare(prefab.name, other.prefab.name);
        }
    }
}