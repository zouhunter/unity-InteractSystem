using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class ElementGroup : MonoBehaviour
    {
        private InstallItem[] _installObjs;
        public UnityAction<InstallItem> onInstall;
        Dictionary<string, List<InstallItem>> objectList = new Dictionary<string, List<InstallItem>>();
        public UnityAction<Dictionary<string, List<InstallItem>>> onAllElementInit;
        void Start()
        {
            _installObjs = GetComponentsInChildren<InstallItem>(true);

            foreach (var item in _installObjs)
            {
                var obj = item;
                if (objectList.ContainsKey(obj.name))
                {
                    objectList[obj.name].Add(obj);
                }
                else
                {
                    objectList[obj.name] = new List<InstallItem>() { obj };
                }

                obj.onInstallOkEvent = () => { onInstall(obj); };
            }
            onAllElementInit.Invoke(objectList);
        }
    }

}
