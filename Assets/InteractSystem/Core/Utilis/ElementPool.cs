using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem
{
    public class ElementPool : ElementPool<ISupportElement>
    {

    }

    public class ElementPool<T> : List<T> where T : ISupportElement
    {
        public UnityAction<T> onAdded { get; set; }
        public UnityAction<T> onRemoved { get; set; }
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

                    if (onAdded != null)
                        onAdded.Invoke(ele);
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

                    if (onRemoved != null)
                        onRemoved.Invoke(ele);
                }
            }
        }
    }
}