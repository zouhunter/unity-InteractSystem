using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class DragStart : MonoBehaviour
    {
        [Range(1, 10)]
        public float distence = 1;
        public float Distence { get { return distence; } set { distence = value; } }

        private DragObj pickedUpObj;
        public UnityAction onInstallOk;

        /// <summary>
        /// 按名称将元素进行记录
        /// </summary>
        Dictionary<string, List<DragObj>> objectList = new Dictionary<string, List<DragObj>>();

        void Start()
        {
            foreach (Transform item in transform)
            {
                DragObj obj = item.GetComponent<DragObj>();
                if (objectList.ContainsKey(obj.name))
                {
                    objectList[obj.name].Add(obj);
                }
                else
                {
                    objectList[obj.name] = new List<DragObj>() { obj };
                }
                obj.onInstallOkEvent = () =>
                {
                    if (onInstallOk != null) onInstallOk();
                };
            }
        }

        /// <summary>
        /// 插入对象
        /// </summary>
        /// <param name="isOn"></param>
        public void InsertScript<T>(bool isOn) where T : MonoBehaviour
        {
            foreach (Transform item in transform)
            {
                T titem = item.gameObject.GetComponent<T>();
                if (isOn && titem == null)
                {
                    item.gameObject.AddComponent<T>();
                }
                else if (!isOn && titem != null)
                {
                    Destroy(titem);
                }
            }
        }

        /// <summary>
        /// 拿起元素
        /// </summary>
        /// <param name="pickedUpObj"></param>
        public bool PickUpObject(DragObj pickedUpObj)
        {
            if (!pickedUpObj.Installed)
            {
                this.pickedUpObj = pickedUpObj;
                pickedUpObj.OnPickUp();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 放下元素
        /// </summary>
        public void PickDownPickedUpObject()
        {
            pickedUpObj.OnPickDown();
        }

        /// <summary>
        /// 是否可以安装到指定坐标（名称条件）
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool CanInstallToPos(DragPos pos)
        {
            return pickedUpObj.name == pos.name;
        }

        /// <summary>
        /// 安装元素到指定坐标
        /// </summary>
        /// <param name="pos"></param>
        public void InstallPickedUpObject(DragPos pos)
        {
            pos.Attach(pickedUpObj);
            pickedUpObj.QuickInstall(pos);
        }

        /// <summary>
        /// 将未安装的元素安装到指定的坐标
        /// </summary>
        /// <param name="posList"></param>
        public void InstallPosListObjects(List<DragPos> posList)
        {
            DragPos pos;
            for (int i = 0; i < posList.Count; i++)
            {
                pos = posList[i];
                DragObj obj = GetUnInstalledObj(pos.name);
                pos.Attach(obj);
                obj.NormalInstall(pos);
            }
        }
        /// <summary>
        /// 快速安装 列表 
        /// </summary>
        /// <param name="posList"></param>
        public void QuickInstallPosListObjects(List<DragPos> posList)
        {
            DragPos pos;
            for (int i = 0; i < posList.Count; i++)
            {
                pos = posList[i];
                if (pos != null)
                {
                    DragObj obj = GetUnInstalledObj(pos.name);
                    obj.QuickInstall(pos);
                    pos.Attach(obj);
                }
            }
        }
        /// <summary>
        /// uninstll
        /// </summary>
        /// <param name="posList"></param>
        public void UnInstallPosListObjects(List<DragPos> posList)
        {
            DragPos pos;
            for (int i = 0; i < posList.Count; i++)
            {
                pos = posList[i];
                DragObj obj = pos.Detach();
                obj.NormalUnInstall();
            }
        }
        /// <summary>
        /// QuickUnInstall
        /// </summary>
        /// <param name="posList"></param>
        public void QuickUnInstallPosListObjects(List<DragPos> posList)
        {
            DragPos pos;
            for (int i = 0; i < posList.Count; i++)
            {
                pos = posList[i];
                DragObj obj = pos.Detach();
                obj.QuickUnInstall();
            }
        }

        /// <summary>
        /// 找出一个没有安装的元素
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        DragObj GetUnInstalledObj(string elementName)
        {
            List<DragObj> listObj;

            if (objectList.TryGetValue(elementName, out listObj))
            {
                for (int i = 0; i < listObj.Count; i++)
                {
                    if (!listObj[i].Installed)
                    {
                        return listObj[i];
                    }
                }
            }
            return null;
        }

        internal void TryHidePosListObjects(List<DragPos> posList)
        {
            for (int i = 0; i < posList.Count; i++)
            {
                DragObj obj = posList[i].obj;
                obj.TryHide();
            }
        }
    }
}
