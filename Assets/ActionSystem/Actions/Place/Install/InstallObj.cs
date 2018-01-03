using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Internal;

namespace WorldActionSystem
{
    /// <summary>
    /// 模拟安装坐标功能
    /// </summary>
    public class InstallObj : PlaceObj
    {
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Install;
            }
        }
        protected override void OnBeforeEnd(bool force)
        {
            base.OnBeforeEnd(force);

            if (!AlreadyPlaced)
            {
                PickUpAbleElement obj = elementCtrl.GetUnInstalledObj(Name);
                Attach(obj);
                obj.QuickInstall(this);
                obj.StepComplete();
            }
        }
        public override void OnUnDoExecute()
        {
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
            if (!Complete){
                OnEndExecute(false);
            }
        }

        protected override void OnUnInstallComplete()
        {
            if(Started)
            {
                if (AlreadyPlaced)
                {
                    var obj = Detach();
                    obj.PickUpAble = true;
                }
                this.obj = null;
            }
        }

        protected override void OnAutoInstall()
        {
            PickUpAbleElement obj = elementCtrl.GetUnInstalledObj(Name);
            Attach(obj);
            obj.StepActive();
            if (Config.quickMoveElement && !ignorePass)
            {
                obj.QuickInstall(this);
            }
            else
            {
                obj.NormalInstall(this);
            }
        }
    }

}