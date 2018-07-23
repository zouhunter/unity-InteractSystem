using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractSystem.Actions
{

    public abstract class PCOperateCtrl<T> where T:new()
    {
        protected static T _instence;
        public static T Instence
        {
            get
            {
                if (_instence == null)
                {
                    _instence = new T();
                }
                return _instence;
            }
        }
        protected CameraController cameraCtrl
        {
            get
            {
                return CameraController.Instence;
            }
        }
        protected Camera viewCamera
        {
            get
            {
                return cameraCtrl.currentCamera;
            }
        }

        public bool needUpdate { get { return Instence is IUpdateAble; } }
        protected CoroutineController coroutineCtrl { get { return CoroutineController.Instence; } }

        public static bool log = false;
        public List<Graph.OperaterNode> lockList = new List<Graph.OperaterNode>();
        public bool Active { get { return lockList.Count > 0; } }
        public virtual void RegistLock(Graph.OperaterNode item)
        {
            if (!lockList.Contains(item))
            {
                lockList.Add(item);
            }
            if (needUpdate)
            {
                coroutineCtrl.RegistFrameAction((Instence as IUpdateAble).Update);
            }
        }
        public virtual void RemoveLock(Graph.OperaterNode item)
        {
            if (lockList.Contains(item))
            {
                lockList.Remove(item);
            }
            if (lockList.Count == 0 && needUpdate)
            {
                coroutineCtrl.RemoveFrameAction((Instence as IUpdateAble).Update);
            }
        }
        protected virtual void SetUserErr(string error)
        {
            Debug.LogWarning(error);
        }
        
    }
}