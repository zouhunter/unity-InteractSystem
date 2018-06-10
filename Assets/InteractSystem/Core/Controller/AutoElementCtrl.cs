using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractSystem
{
    public class AutoElementCtrl
    {
        private AutoPrefabItem[] autoPrefabItems;
        private List<GameObject> created = new List<GameObject>();
        private Transform parent;

        public AutoElementCtrl(Transform parent,AutoPrefabItem[] autoPrefabItems)
        {
            this.parent = parent;
            this.autoPrefabItems = autoPrefabItems;
        }

        public void Create()
        {
            foreach (var item in autoPrefabItems)
            {
                if (!item.ignore && item.prefab != null)
                {
                    created.Add(CreateOne(item));
                }
            }
        }

        public GameObject CreateOne(AutoPrefabItem item)
        {
            var instence = GameObject.Instantiate(item.prefab);
            instence.transform.SetParent(parent);
            TransUtil.LoadCoordinatesInfo(item.coordinate, instence.transform);
            return instence;
        }

        public void Clear()
        {
            foreach (var item in created)
            {
                if(item)
                GameObject.Destroy(item);
            }
            created.Clear();
        }
    }

}