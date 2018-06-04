using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem.Enviroment
{

    public class EnviromentObj : ScriptableObject, System.IComparable<EnviromentObj>
    {
#if UNITY_EDITOR
        [HideInInspector]
        public int instanceID;
#endif
        /// <summary>
        /// 运行时的实例
        /// </summary>
        private GameObject _instence;
        public GameObject Body
        {
            get
            {
                if (_instence == null)
                {
                    _instence = Instantiate(prefab);
                }
                return _instence;
            }
        }

        public GameObject prefab;
        public int CompareTo(EnviromentObj other)
        {
            if (prefab == null || other.prefab == null) return 0;
            return string.Compare(prefab.name, other.prefab.name);
        }
    }
}