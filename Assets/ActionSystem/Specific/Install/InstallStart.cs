using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    /// <summary>
    /// 记录安装对象,并操作对象
    /// </summary>
    public class InstallStart : MonoBehaviour, IInstallStart
    {
        [Range(1, 10)]
        public float distence = 1;
        public float Distence { get { return distence; } set { distence = value; } }

        private InstallObj pickedUpObj;
        /// <summary>
        /// 按名称将元素进行记录
        /// </summary>
        Dictionary<string, List<InstallObj>> objectList = new Dictionary<string, List<InstallObj>>();

        void Start()
        {
            foreach (Transform item in transform)
            {
                InstallObj obj = item.GetComponent<InstallObj>();
                if (objectList.ContainsKey(obj.name))
                {
                    objectList[obj.name].Add(obj);
                }
                else
                {
                    objectList[obj.name] = new List<InstallObj>() { obj };
                }
            }
        }

        /// <summary>
        /// 插入对象
        /// </summary>
        /// <param name="isOn"></param>
        public void InsertScript<T>(bool isOn) where T:MonoBehaviour
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
        public bool PickUpObject(InstallObj pickedUpObj)
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
        public bool CanInstallToPos(InstallPos pos)
        {
            return pickedUpObj.name == pos.name;
        }

        /// <summary>
        /// 安装元素到指定坐标
        /// </summary>
        /// <param name="pos"></param>
        public void InstallPickedUpObject(InstallPos pos)
        {
            pickedUpObj.NormalInstall(pos);
            pos.Attach(pickedUpObj);
        }
        /// <summary>
        /// 快速安装元素到指定坐标
        /// </summary>
        /// <param name="pos"></param>
        public void QuickInstallPickedUpObject(InstallPos pos)
        {
            pickedUpObj.QuickInstall(pos);
            pos.Attach(pickedUpObj);
        }

        /// <summary>
        /// 将未安装的元素安装到指定的坐标
        /// </summary>
        /// <param name="posList"></param>
        public void InstallPosListObjects(List<InstallPos> posList)
        {
            InstallPos pos;
            for (int i = 0; i < posList.Count; i++)
            {
                pos = posList[i];
                InstallObj obj = GetUnInstalledObj(pos.name);
                obj.NormalInstall(pos);
                pos.Attach(obj);
            }
        }
        /// <summary>
        /// 快速安装 列表 
        /// </summary>
        /// <param name="posList"></param>
        public void QuickInstallPosListObjects(List<InstallPos> posList)
        {
            InstallPos pos;
            for (int i = 0; i < posList.Count; i++)
            {
                pos = posList[i];
                if (pos != null)
                {
                    InstallObj obj = GetUnInstalledObj(pos.name);
                    obj.QuickInstall(pos);
                    pos.Attach(obj);
                }
            }
        }
        /// <summary>
        /// uninstll
        /// </summary>
        /// <param name="posList"></param>
        public void UnInstallPosListObjects(List<InstallPos> posList)
        {
            InstallPos pos;
            for (int i = 0; i < posList.Count; i++)
            {
                pos = posList[i];
                InstallObj obj = pos.Detach();
                obj.NormalUnInstall();
            }
        }
        /// <summary>
        /// QuickUnInstall
        /// </summary>
        /// <param name="posList"></param>
        public void QuickUnInstallPosListObjects(List<InstallPos> posList)
        {
            InstallPos pos;
            for (int i = 0; i < posList.Count; i++)
            {
                pos = posList[i];
                InstallObj obj = pos.Detach();
                obj.QuickUnInstall();
            }
        }

        /// <summary>
        /// 找出一个没有安装的元素
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        InstallObj GetUnInstalledObj(string elementName)
        {
            List<InstallObj> listObj;

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

    }

}