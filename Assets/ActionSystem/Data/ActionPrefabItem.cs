using UnityEngine;
using System;
namespace WorldActionSystem
{
    [System.Serializable]
    public class ActionPrefabItem:IComparable<ActionPrefabItem>
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
                    var name = prefab == null ? "Null" : prefab.name;
                    if (!rematrix)
                    {
                        _id = name;
                    }
                    else
                    {
                        _id = string.Format("[{0}][{1}]", name, matrix);
                    }
                    Debug.Log(_id);
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
        public int CompareTo(ActionPrefabItem other)
        {
            if (prefab == null || other.prefab == null) return 0;
            return string.Compare(prefab.name,other.prefab.name);
        }
    }

}