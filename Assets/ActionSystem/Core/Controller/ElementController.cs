using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WorldActionSystem
{
    public class ElementController
    {
        public static ElementController _instence;
        public static ElementController Instence
        {
            get
            {
                if (_instence == null)
                {
                    _instence = new ElementController();
                }
                return _instence;
            }
        }

        public static bool log = true;
        public event UnityAction<ISupportElement> onRegistElememt;
        public event UnityAction<ISupportElement> onRemoveElememt;
        //所有元素列表
        private ElementPool elementList = new ElementPool();
        //动态元素预制体
        private ElementPool runTimeElementPrefabs = new ElementPool();
        //动态创建的元素
        private ElementPool rutimeCreatedList = new ElementPool();
        //优先查看的列表
        private ElementPool priorityList = new ElementPool();
        //元素锁
        private Dictionary<ISupportElement, List<object>> lockDic = new Dictionary<ISupportElement, List<object>>();

        private ElementController() { }

        /// <summary>
        /// 外部添加Element
        /// </summary>
        public void RegistElement(params ISupportElement[] items)
        {
            foreach (var item in items)
            {
                if (!elementList.Contains(item))
                {
                    elementList.ScureAdd(item);
                    if (onRegistElememt != null)
                    {
                        onRegistElememt.Invoke(item);
                    }
                }
            }
        }

        /// <summary>
        /// 移除Eelement
        /// </summary>
        /// <param name="item"></param>
        public void RemoveElement(ISupportElement item)
        {
            if (elementList.Contains(item))
            {
                elementList.ScureRemove(item);

                if (onRemoveElememt != null)
                {
                    onRemoveElememt.Invoke(item);
                }
            }
        }

        /// <summary>
        /// 清除所有动态创建的元素
        /// </summary>
        public void ClearExtraCreated()
        {
            var list = rutimeCreatedList.ToArray();
            foreach (var item in list)
            {
                if (TryDestroyElement(item))
                {
                    rutimeCreatedList.ScureRemove(item);
                }
            }
        }

        internal void SetPriority(ActionItem[] subActions)
        {
            if (log)
            {
                var str = "SetPriority:";
                foreach (var item in subActions)
                {
                    str += item;
                }
                Debug.Log(str);
            }
            priorityList.Clear();
            priorityList.AddRange(subActions);
        }

        /// <summary>
        /// 判断元素是否被占用
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool IsLocked(ISupportElement item)
        {
            return lockDic.ContainsKey(item) &&
            lockDic[item] != null &&
            lockDic[item].Count > 0;
        }

        /// <summary>
        /// 元素加锁
        /// </summary>
        /// <param name="item"></param>
        public void LockElement(ISupportElement item, object lk)
        {
            if (lockDic.ContainsKey(item))
            {
                if (!lockDic[item].Contains(lk))
                {
                    lockDic[item].Add(lk);
                }
            }
            else
            {
                lockDic[item] = new List<object>() { lk };
            }
        }

        /// <summary>
        /// 元素解锁
        /// </summary>
        /// <param name="item"></param>
        public bool UnLockElement(ISupportElement item, object lk)
        {
            if (lockDic.ContainsKey(item) && lockDic[item].Contains(lk))
            {
                lockDic[item].Remove(lk);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 尝试销毁一个元素
        /// </summary>
        /// <param name="item"></param>
        public bool TryDestroyElement(ISupportElement item)
        {
            if (!IsLocked(item))
            {
                RemoveElement(item);
                if (item.Body != null)
                {
                    UnityEngine.Object.DestroyImmediate(item.Body);
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 尝试创建一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public T TryCreateElement<T>(string elementName, Transform parent, bool regist = true) where T : ISupportElement
        {
            T element = default(T);
            var prefab = runTimeElementPrefabs.Find(x => x.Name == elementName);
            if (prefab != null)
            {
                var e = CreateElement(prefab, parent);
                e.Name = prefab.Name;
                if (e is T)
                {
                    element = (T)e;
                    element.IsRuntimeCreated = true;
                    if (regist)
                    {
                        rutimeCreatedList.ScureAdd(element);
                    }
                }
                else
                {
                    Debug.LogError(e + "is not an " + typeof(T).FullName);
                }
            }
            else
            {
                Debug.Log("prefab not exist:" + elementName);
            }
            return element;
        }
        /// <summary>
        /// 手动注册
        /// </summary>
        /// <param name="element"></param>
        public void RegistRuntimeCreated(ISupportElement element)
        {
            rutimeCreatedList.ScureAdd(element);
        }

        private ISupportElement CreateElement(ISupportElement prefab, Transform parent)
        {
            var instence = UnityEngine.Object.Instantiate(prefab.Body);
            instence.transform.SetParent(parent);
            instence.gameObject.SetActive(true);
            var element = instence.GetComponent<ISupportElement>();
            element.IsRuntimeCreated = true;
            return element;
        }

        internal void RemoveRunTimeElements(IList<RunTimePrefabItem> elements)
        {
            foreach (var item in elements)
            {
                if (item == null && item.prefab) continue;
                var element = item.prefab.GetComponent<ISupportElement>();
                if (element != null && runTimeElementPrefabs.Contains(element))
                {
                    runTimeElementPrefabs.ScureRemove(element);
                }
            }
        }

        internal void RegistRunTimeElements(IList<RunTimePrefabItem> elements)
        {
            foreach (var item in elements)
            {
                if (item == null && item.prefab) continue;
                var element = item.prefab.GetComponent<ISupportElement>();
                if (element != null && !runTimeElementPrefabs.Contains(element))
                {
                    runTimeElementPrefabs.ScureAdd(element);
                }
            }
        }

        /// <summary>
        /// 获取指定元素名的列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elementName"></param>
        /// <param name="priorityList"></param>
        /// <returns></returns>
        public List<T> GetElements<T>(string elementName, bool priorit) where T : ISupportElement
        {
            List<ISupportElement> list = new List<ISupportElement>();
            if (priorit)
            {
                list = priorityList.FindAll(x => x.Name == elementName && x is T);
            }

            var otherList = elementList.FindAll(x => x.Name == elementName && x is T);

            list.AddRange(otherList.Where(x=>!list.Contains(x)));

            if (list.Count > 0)
            {
                return list.ConvertAll<T>(x => (T)x);
            }
            return null;

        }

        /// <summary>
        /// 获取所有T的类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetElements<T>()
        {
            var list = elementList.FindAll(x => x is T);
            if (list.Count > 0)
            {
                return list.ConvertAll<T>(x => (T)x);
            }
            Debug.LogWarning("配制错误,缺少" + typeof(T).ToString());
            return null;
        }
    }

}
