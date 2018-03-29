using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem
{
    public class ElementPool : ElementPool<ISupportElement> {

    }

    public class ElementPool<T> : List<T> where T : ISupportElement
    {
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
                }
            }
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="element"></param>
        public void ScureRemove(params T[] elements)
        {
            foreach (var ele in elements)
            {
                if (this.Contains(ele))
                {
                    this.Remove(ele);
                }
            }
        }
    }
}