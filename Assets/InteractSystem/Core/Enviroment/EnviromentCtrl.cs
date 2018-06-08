using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Enviroment
{
    /// <summary>
    /// 在指定的坐标只会存在一个对象实例
    /// </summary>
    public class EnviromentCtrl
    {
        public static bool log = false;
        private ActionSystem Context { get { return ActionSystem.Instence; } }
        private List<EnviromentItem> oringalItems = new List<EnviromentItem>();
        private Dictionary<string, EnviromentItem> environmentDic = new Dictionary<string, EnviromentItem>();
        private static EnviromentCtrl _intence;
        public static EnviromentCtrl Instence
        {
            get
            {
                if (_intence == null)
                    _intence = new EnviromentCtrl();
                return _intence;
            }
        }

        public void RegistElements(Enviroment.EnviromentItem[] environments)
        {
            oringalItems.AddRange(environments);
        }

        public void RemoveElements(Enviroment.EnviromentItem[] environments)
        {
            foreach (var item in environments)
            {
                if(oringalItems.Contains(item))
                {
                    oringalItems.Remove(item);
                }
            }
        }

        internal void OrignalState(EnviromentInfo[] enviromentItems)
        {
            //设置环境为初始状态
            foreach (var item in enviromentItems)
            {
                if (item.ignore) continue;
                var enviroment = SurchItem(item);
                SetEnviroment(enviroment, item.coordinate, item.originalState);
            }
        }

        internal void StartState(EnviromentInfo[] enviromentItems)
        {
            //设置环境为激活状态
            foreach (var item in enviromentItems)
            {
                if (item.ignore) continue;
                var enviroment = SurchItem(item);
                if (enviroment != null)
                {
                    SetEnviroment(enviroment, item.coordinate, item.startState);
                }
            }
        }
        internal void CompleteState(EnviromentInfo[] enviromentItems)
        {
            //设置环境为结束状态
            foreach (var item in enviromentItems)
            {
                if (item.ignore) continue;
                var enviroment = SurchItem(item);
                if (enviroment != null)
                {
                    SetEnviroment(enviroment, item.coordinate, item.completeState);
                }
            }
        }

        private EnviromentItem SurchItem(EnviromentInfo info)
        {
            if (!environmentDic.ContainsKey(info.ID))
            {
                var item = oringalItems.Find(x => x.Name == info.enviromentName);
                if (item != null)
                {
                    environmentDic[info.ID] = item.CreateCopy();
                }
                else
                {
                    Debug.LogError("缺少环境配制 :" + info.enviromentName);
                }
            }
            return environmentDic[info.ID];
        }

        private void SetEnviroment(EnviromentItem obj, Coordinate coordinates, bool active)
        {
            if(log) Debug.Log("SetEnviroment:" + obj.prefab + " -> " + active);
            if (obj.Created || active)//如果没有创建并且当前不需要显示,略过
            {
                if (obj.Body != null)
                {
                    obj.Body.transform.SetParent(Context.transform);
                    TransUtil.LoadCoordinatesInfo(coordinates, obj.Body.transform);
                    obj.Body.SetActive(active);
                }
            }

        }
    }
}
