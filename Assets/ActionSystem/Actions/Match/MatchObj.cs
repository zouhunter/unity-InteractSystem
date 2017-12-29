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
    public class MatchObj : PlaceObj
    {
        public bool completeMoveBack = true;//结束时退回
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Match;
            }
        }

        public bool Matched { get { return obj != null; } }

        public override int layer { get { return Layers.matchPosLayer; } }

        protected override void OnBeforeComplete(bool force)
        {
            base.OnBeforeComplete(force);
            if (Matched && completeMoveBack){
                obj.QuickUnInstall();
            }
        }
        protected override void OnBeforeUnDo()
        {
            base.OnBeforeUnDo();
            if (Matched){
                var obj = Detach();
                obj.QuickUnInstall();
            }
        }
        protected override void OnAutoInstall()
        {
            var obj = elementCtrl.GetUnInstalledObj(Name);
            Attach(obj);
            if (Config.quickMoveElement && !ignorePass)
            {
                if (!completeMoveBack)
                {
                    obj.QuickInstall(this,false,false);
                }
                else
                {
                    OnInstallComplete();
                }
            }
            else
            {
                obj.NormalInstall(this, false,false);
            }
        }

        protected override void OnInstallComplete()
        {
            if (!Complete)
            {
                OnEndExecute(false);
            }
        }
    }
}