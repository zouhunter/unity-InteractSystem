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
    public class InstallStart : MonoBehaviour
    {
        [Range(1, 10)]
        public float distence = 1;
        public float Distence { get { return distence; } set { distence = value; } }
        public InstallObj[] InstallObjs { get { return _installObjs; } }
        public UnityAction<InstallObj> onInstall;


        private InstallObj pickedUpObj;
        private InstallObj[] _installObjs;
        /// <summary>
        /// 按名称将元素进行记录
        /// </summary>
        Dictionary<string, List<InstallObj>> objectList = new Dictionary<string, List<InstallObj>>();

        void Start()
        {
            _installObjs = GetComponentsInChildren<InstallObj>(true);

            foreach (var item in _installObjs)
            {
                var obj = item;
                if (objectList.ContainsKey(obj.name))
                {
                    objectList[obj.name].Add(obj);
                }
                else
                {
                    objectList[obj.name] = new List<InstallObj>() { obj };
                }

                obj.onInstallOkEvent = () => { onInstall(obj); };
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
            pos.Attach(pickedUpObj);
            pickedUpObj.QuickInstall(pos);
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
                pos.Attach(obj);
                obj.NormalInstall(pos);
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
            foreach (var item in posList)
            {
                InstallObj obj = item.Detach();
                obj.QuickUnInstall();
            }
        }
        /// <summary>
        /// 激活步骤 
        /// </summary>
        /// <param name="poss"></param>
        public void SetStartNotify(List<InstallPos> posList)
        {
            List<InstallObj> temp = new List<InstallObj>();
            foreach (var pos in posList)
            {
                List<InstallObj> listObjs;
                if (objectList.TryGetValue(pos.name, out listObjs))
                {
                    for (int j = 0; j < listObjs.Count; j++)
                    {
                        if(!listObjs[j].Installed &&!temp.Contains(listObjs[j]))
                        {
                            temp.Add(listObjs[j]);
                            listObjs[j].StepActive();
                            break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 结束指定步骤
        /// </summary>
        /// <param name="poss"></param>
        public void SetCompleteNotify(List<InstallPos> poss)
        {
            //当前步骤结束
            foreach (var item in poss)
            {
                item.obj.StepComplete();
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