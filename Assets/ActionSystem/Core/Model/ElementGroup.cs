using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class ElementGroup : ScriptableObject
    {
        [SerializeField]//用户创建元素
        public List<RunTimePrefabItem> runTimeElements = new List<RunTimePrefabItem>();
        [SerializeField]//自动创建元素
        protected List<AutoPrefabItem> autoElements = new List<AutoPrefabItem>();

        private void OnEnable()
        {
            ElementController.Instence.RegistRunTimeElements(runTimeElements);
        }
        private void OnDestroy()
        {
            ElementController.Instence.RemoveRunTimeElements(runTimeElements);
        }
    }
}
