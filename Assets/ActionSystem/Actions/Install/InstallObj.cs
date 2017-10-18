using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
#if !NoFunction
using DG.Tweening;
#endif
namespace WorldActionSystem
{
    /// <summary>
    /// 模拟安装坐标功能
    /// </summary>
    public class InstallObj :ActionObj
    {
        public bool autoInstall;
        public bool Installed { get { return obj != null; } }
        public PickUpAbleElement obj { get; private set; }

        void Awake(){
            gameObject.layer = Setting.installPosLayer;
        }

        public bool Attach(PickUpAbleElement obj)
        {
            if(this.obj != null)
            {
                return false;
            }
            else
            {
                this.obj = obj;
                return true;
            }
        }

        public PickUpAbleElement Detach()
        {
            PickUpAbleElement old = obj;
            obj = null;
            return old;
        }
    }

}