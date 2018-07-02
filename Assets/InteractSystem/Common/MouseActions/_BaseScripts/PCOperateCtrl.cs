using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractSystem.Actions
{

    public abstract class PCOperateCtrl<T> : OperateCtrl<T> where T : PCOperateCtrl<T>, new() 
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
        public override void RegistLock(Graph.OperaterNode item)
        {
            base.RegistLock(item);
            if (needUpdate)
            {
                coroutineCtrl.RegistFrameAction((Instence as IUpdateAble).Update);
            }
        }
        public override void RemoveLock(Graph.OperaterNode item)
        {
            base.RemoveLock(item);
            if (lockList.Count == 0 && needUpdate)
            {
                coroutineCtrl.RemoveFrameAction((Instence as IUpdateAble).Update);
            }
        }
    }
}