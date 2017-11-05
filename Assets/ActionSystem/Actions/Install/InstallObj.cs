using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Internal;
#if !NoFunction
using DG.Tweening;
#endif
namespace WorldActionSystem
{
    /// <summary>
    /// 模拟安装坐标功能
    /// </summary>
    public class InstallObj : PlaceObj
    {
        public override int layer
        {
            get
            {
               return Setting.installPosLayer;
            }
        }

        public override void OnEndExecute(bool force)
        {
            base.OnEndExecute(force);

            if (!AlreadyPlaced)
            {
                PickUpAbleElement obj = ElementController.GetUnInstalledObj(Name);
                Attach(obj);
                obj.QuickInstall(this);
                obj.StepComplete();
            }
        }

        public override void OnUnDoExecute()
        {
            Debug.Log("UnDo:" + Name);

            base.OnUnDoExecute();
            if (AlreadyPlaced)
            {
                var obj = Detach();
                obj.QuickUnInstall();
                obj.StepUnDo();
            }
        }

        protected override void OnInstallComplete()
        {
            OnEndExecute(false);
        }

        protected override void OnUnInstallComplete()
        {
            this.obj = null;
            OnUnDoExecute();
            OnStartExecute(auto);
        }

        protected override void OnAutoInstall()
        {
            PickUpAbleElement obj = ElementController.GetUnInstalledObj(Name);
            Attach(obj);
            obj.StepActive();
            if (Setting.ignoreInstall && !ignorePass)
            {
                if (!hideOnInstall)
                {
                    obj.QuickInstall(this);
                }
                else
                {
                    OnInstallComplete();
                }
            }
            else
            {
                obj.NormalInstall(this);
            }
        }
    }

}