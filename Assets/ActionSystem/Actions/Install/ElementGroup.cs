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
        private InstallItem[] _installItems;
        public event UnityAction onInstall;
        Dictionary<string, List<InstallItem>> objectList = new Dictionary<string, List<InstallItem>>();
        public InstallItem[] InstallItems { get { return _installItems; } }
        private InstallItem pickedUpObj;

        void Start()
        {
            _installItems = GetComponentsInChildren<InstallItem>(true);

            foreach (var item in _installItems)
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

                obj.onInstallOkEvent = OnInstallOK;
            }
        }

        /// <summary>
        /// 拿起元素
        /// </summary>
        /// <param name="pickedUpObj"></param>
        public bool PickUpObject(InstallItem pickedUpObj)
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
        public bool CanInstallToPos(InstallObj pos)
        {
            return pickedUpObj.name == pos.name;
        }

        /// <summary>
        /// 安装元素到指定坐标
        /// </summary>
        /// <param name="pos"></param>
        public void InstallPickedUpObject(InstallObj pos)
        {
            pos.Attach(pickedUpObj);
            pickedUpObj.QuickInstall(pos);
        }

        /// <summary>
        /// 将未安装的元素安装到指定的坐标
        /// </summary>
        /// <param name="posList"></param>
        public void InstallObjListObjects(List<InstallObj> posList)
        {
            InstallObj pos;
            for (int i = 0; i < posList.Count; i++)
            {
                pos = posList[i];
                InstallItem obj = GetUnInstalledObj(pos.name);
                pos.Attach(obj);
                obj.NormalInstall(pos);
            }
        }
        /// <summary>
        /// 快速安装 列表 
        /// </summary>
        /// <param name="posList"></param>
        public void QuickInstallObjListObjects(List<InstallObj> posList)
        {
            InstallObj pos;
            for (int i = 0; i < posList.Count; i++)
            {
                pos = posList[i];
                if (pos != null)
                {
                    InstallItem obj = GetUnInstalledObj(pos.name);
                    obj.QuickInstall(pos);
                    pos.Attach(obj);
                }
            }
        }
        /// <summary>
        /// uninstll
        /// </summary>
        /// <param name="posList"></param>
        public void UnInstallObjListObjects(List<InstallObj> posList)
        {
            InstallObj pos;
            for (int i = 0; i < posList.Count; i++)
            {
                pos = posList[i];
                InstallItem obj = pos.Detach();
                obj.NormalUnInstall();
            }
        }
        /// <summary>
        /// QuickUnInstall
        /// </summary>
        /// <param name="posList"></param>
        public void QuickUnInstallObjListObjects(List<InstallObj> posList)
        {
            foreach (var item in posList)
            {
                InstallItem obj = item.Detach();
                obj.QuickUnInstall();
            }
        }
        /// <summary>
        /// 激活步骤 
        /// </summary>
        /// <param name="poss"></param>
        public void SetStartNotify(List<InstallObj> posList)
        {
            List<InstallItem> temp = new List<InstallItem>();
            foreach (var pos in posList)
            {
                List<InstallItem> listObjs;
                if (objectList.TryGetValue(pos.name, out listObjs))
                {
                    for (int j = 0; j < listObjs.Count; j++)
                    {
                        if (!listObjs[j].Installed && !temp.Contains(listObjs[j]))
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
        public void SetCompleteNotify(List<InstallObj> poss)
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
        InstallItem GetUnInstalledObj(string elementName)
        {
            List<InstallItem> listObj;

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

        private void OnInstallOK()
        {
            if (onInstall != null) onInstall.Invoke();
        }

    }

}
