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
    public class InstallPos :MonoBehaviour
    {
        public string stapName;
        public bool autoInstall;

        public bool Installed { get { return obj != null; } }
        public InstallObj obj { get; private set; }

        private void Start()
        {
            gameObject.layer = LayerMask.NameToLayer(Setting.installPosLayer);
        }
        //public IInstallCtrl installCtrl { private get; set; }

        public void Attach(InstallObj obj)
        {
            this.obj = obj;
        }

        public InstallObj Detach()
        {
            InstallObj old = obj;
            obj = null;
            return old;
        }
    }

}