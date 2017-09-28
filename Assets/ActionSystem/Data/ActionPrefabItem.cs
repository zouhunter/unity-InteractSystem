using UnityEngine;
using System;
namespace WorldActionSystem
{
    [System.Serializable]
    public class ActionPrefabItem:IComparable<string>
    {
#if UNITY_EDITOR
        public int instanceID;
#endif
        protected string _id;
        public string ID
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    var name = prefab == null ? "Null" : prefab.name;
                    if (!reset)
                    {
                        _id = name;
                    }
                    else
                    {
                        _id = string.Format("[{0}][{1}]", name, ((target == null) ? "" : target.GetHashCode().ToString()));
                    }
                }
                return _id;
            }
        }
        public bool containsCommand;
        public bool containsPickAble;
        public bool reset;
        public Transform target;
        public GameObject prefab;

        public int CompareTo(string other)
        {
            return string.Compare(prefab.name,other);
        }
    }

}