using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractSystem
{

    public abstract class OperateCtrl<T,A> where T :OperateCtrl<T,A> where A:ActionItem
    {
        public static bool log = false;
        public List<A> itemList = new List<A>();
        public bool Active { get { return itemList.Count > 0; } }
        public virtual void RegistItem(A item)
        {
            if (!itemList.Contains(item))
            {
                itemList.Add(item);
            }
        }
        public virtual void RemoveItem(A item)
        {
            if (itemList.Contains(item))
            {
                itemList.Remove(item);
            }
        }
        protected virtual void SetUserErr(string error)
        {
            Debug.Log(error);
        }
    }
}