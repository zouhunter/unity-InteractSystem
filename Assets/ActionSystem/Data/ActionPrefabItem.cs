using UnityEngine;
using System;
namespace WorldActionSystem
{
    [System.Serializable]
    public class AutoPrefabItem:IComparable<AutoPrefabItem>
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
                    _id = CalcuteID(prefab, rematrix, matrix);
                }
                return _id;
            }
        }
        public bool containsCommand;
        public bool containsPickup;
        public bool rematrix;
        public Matrix4x4 matrix;
        public GameObject prefab;
        public bool ignore;

        public static string CalcuteID(GameObject prefab,bool rematrix, Matrix4x4 matrix)
        {
            string _id = null;
            var name = prefab == null ? "Null" : prefab.name;
            if (!rematrix)
            {
                _id = name;
            }
            else
            {
                _id = string.Format("[{0}][{1}]", name, matrix);
            }
            return _id;
        }
        public int CompareTo(AutoPrefabItem other)
        {
            if (prefab == null || other.prefab == null) return 0;
            return string.Compare(prefab.name,other.prefab.name);
        }
    }

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
        public GameObject prefab;

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