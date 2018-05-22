using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem
{
    public class ElementPool : ElementPool<ISupportElement>
    {

    }

    public class ElementPool<T> : List<T> where T : ISupportElement
    {
        public UnityAction<T> onAdd { get; set; }
        public UnityAction<T> onRemove { get; set; }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="elements"></param>
        public void ScureAdd(params T[] elements)
        {
            foreach (var ele in elements)
            {
                if (!this.Contains(ele))
                {
                    this.Add(ele);
                    if (onAdd != null)
                        onAdd.Invoke(ele);
                }
            }
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="elements"></param>
        public void ScureRemove(params T[] elements)
        {
            foreach (var ele in elements)
            {
                if (this.Contains(ele))
                {
                    this.Remove(ele);
                    if (onRemove != null)
                        onRemove.Invoke(ele);
                }
            }
        }
    }
}