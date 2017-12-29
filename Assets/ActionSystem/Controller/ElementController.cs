using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class ElementController
    {
        public static ElementController _instence;
        private ElementController() { }
        public static ElementController Instence
        {
            get
            {
                if(_instence == null)
                {
                    _instence = new ElementController();
                }
                return _instence;
            }
        }

        private Dictionary<string, List<PickUpAbleElement>> objectList = new Dictionary<string, List<PickUpAbleElement>>();
        private List<PlaceObj> lockQueue = new List<PlaceObj>();
        public bool log = false;

        /// <summary>
        /// 外部添加Element
        /// </summary>
        public void RegistElement(PickUpAbleElement item)
        {
            if (objectList.ContainsKey((string)item.Name))
            {
                objectList[(string)item.Name].Add((PickUpAbleElement)item);
            }
            else
            {
                objectList[(string)item.Name] = new System.Collections.Generic.List<PickUpAbleElement>() { item };
            }
        }
        public void RemoveElement(PickUpAbleElement item)
        {
            foreach (var objItem in objectList)
            {
                if(item.Name == objItem.Key)
                {
                    if(objItem.Value.Contains(item))
                    {
                        objItem.Value.Remove(item);
                    }
                }
            }
        }

        /// <summary>
        /// 获取指定元素名的列表
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public List<PickUpAbleElement> GetElements(string elementName)
        {
            if (objectList.ContainsKey(elementName))
            {
                return objectList[elementName];
            }
            else
            {
                Debug.LogWarning("配制错误,缺少" + elementName);
                return null;
            }
        }

        /// <summary>
        /// 找出一个没有安装的元素
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public PickUpAbleElement GetUnInstalledObj(string elementName)
        {
            List<PickUpAbleElement> listObj;

            if (objectList.TryGetValue(elementName, out listObj))
            {
                for (int i = 0; i < listObj.Count; i++)
                {
                    if (!listObj[i].HaveBinding)
                    {
                        return listObj[i];
                    }
                }
            }
            throw new Exception("配制错误,缺少" + elementName);
        }

        public void ActiveElements(PlaceObj element)
        {

            var actived = lockQueue.Find(x => x.Name == element.name);
            if (actived == null)
            {
                var objs = GetElements(element.Name);
                if (objs == null) return;
                for (int i = 0; i < objs.Count; i++)
                {
                    if (log) Debug.Log("ActiveElements:" + element.Name + (!objs[i].Started && !objs[i].HaveBinding));

                    if (!objs[i].Started && !objs[i].HaveBinding)
                    {
                        objs[i].StepActive();
                    }
                }
            }
            lockQueue.Add(element);
        }
        public void CompleteElements(PlaceObj element, bool undo)
        {
            lockQueue.Remove(element);
            var active = lockQueue.Find(x => x.Name == element.Name);
            if (active == null)
            {
                var objs = GetElements(element.Name);
                if (objs == null) return;
                for (int i = 0; i < objs.Count; i++)
                {
                    if (log) Debug.Log("CompleteElements:" + element.Name + objs[i].Started);

                    if (objs[i].Started)
                    {
                        if (undo)
                        {
                            objs[i].StepUnDo();
                        }
                        else
                        {
                            objs[i].StepComplete();
                        }
                    }
                }
            }


        }
    }

}
