using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractSystem
{

    public abstract class OperateCtrl<T> where T :OperateCtrl<T> 
    {
        public static bool log = false;
        public List<Graph.OperaterNode> lockList = new List<Graph.OperaterNode>();
        public bool Active { get { return lockList.Count > 0; } }
        public virtual void RegistLock(Graph.OperaterNode item)
        {
            if (!lockList.Contains(item))
            {
                lockList.Add(item);
            }
        }
        public virtual void RemoveLock(Graph.OperaterNode item)
        {
            if (lockList.Contains(item))
            {
                lockList.Remove(item);
            }
        }
        protected virtual void SetUserErr(string error)
        {
            Debug.LogError(error);
        }
    }
}