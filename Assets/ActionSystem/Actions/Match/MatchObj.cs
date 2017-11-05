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

        public bool Matched { get { return obj != null; } }

        public override int layer { get { return Setting.matchPosLayer; } }

       
        public override void OnEndExecute(bool force)
        {
            base.OnEndExecute(force);

            if (!Matched){
                PickUpAbleElement obj = ElementController.GetUnInstalledObj(Name);
                Attach(obj);
            }

            if (completeMoveBack){
                this.obj.QuickUnInstall();
            }
        }

        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            if (Matched){
               var obj = Detach();
                obj.QuickUnInstall();
            }
        }
    
        protected override void OnAutoInstall()
        {
            var obj = ElementController.GetUnInstalledObj(Name);
            Attach(obj);
            if (Setting.ignoreMatch && !ignorePass)
            {
                if (!completeMoveBack)
                {
                    obj.QuickInstall(this,true,false);
                }
                else
                {
                    OnInstallComplete(obj);
                }
            }
            else
            {
                obj.NormalInstall(this, false,false);
            }
        }

        protected override void OnInstallComplete(PickUpAbleElement arg0)
        {
            if (arg0 == this.obj && Started && !Complete){
                OnEndExecute(false);
            }
        }

        protected override void OnUnInstallComplete(PickUpAbleElement arg0)
        {
            if(arg0 == this.obj && Started)
            {
                arg0.StepUnDo();
            }
        }
    }
}