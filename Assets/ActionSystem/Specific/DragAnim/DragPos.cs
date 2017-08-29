using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
#if !NoFunction
using DG.Tweening;
#endif
namespace WorldActionSystem
{
    public class DragPos : MonoBehaviour, ActionObj
    {
        public string stapName;
        public bool autoInstall;

        public bool Installed { get { return obj != null; } }
        public DragObj obj { get; private set; }

        public IRemoteController RemoteCtrl
        {
            get
            {
                return ActionSystem.Instance.RemoteController;
            }
        }

        public void Attach(DragObj obj)
        {
            this.obj = obj;
        }

        public DragObj Detach()
        {
            DragObj old = obj;
            obj = null;
            return old;
        }
    }
}