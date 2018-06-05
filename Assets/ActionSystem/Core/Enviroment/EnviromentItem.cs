using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem.Enviroment
{

    [System.Serializable]
    public class EnviromentItem
    {
#if UNITY_EDITOR
        [HideInInspector]
        public int instanceID;
#endif
        [SerializeField]
        private string _name;
        public string Name {
            get {
                if (string.IsNullOrEmpty(_name))
                {
                    if(prefab != null)
                    {
                        return prefab.name;
                    }
                }
                return _name;
            }
            set
            {
                _name = value;
            }
        }
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
                    _instence =Object. Instantiate(prefab);
                    _instence.name = prefab.name;
                }
                return _instence;
            }
        }
        public GameObject prefab;
        public bool Created { get { return _instence != null; } }

        public EnviromentItem CreateCopy()
        {
            var item = new EnviromentItem();
            item.prefab = prefab;
            item.Name = Name;
            return item;
        }
    }
}