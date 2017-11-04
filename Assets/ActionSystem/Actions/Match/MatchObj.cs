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
                this.obj.QuickMoveBack();
            }
        }

        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            if (Matched){
               var obj = Detach();
                obj.QuickMoveBack();
            }
        }
    
        protected override void OnAutoInstall()
        {
            if (Setting.ignoreMatch && !ignorePass)
            {
                if (!completeMoveBack)
                {
                    obj.QuickMoveTo(gameObject);
                }
                else
                {
                    OnInstallComplete(obj);
                }
            }
            else
            {
                obj.StraightMove = straightMove;
                obj.IgnoreMiddle = ignoreMiddle;
                obj.Passby = passBy;
                obj.NormalMoveTo(gameObject, false);
            }
        }

        protected override void OnInstallComplete(PickUpAbleElement arg0)
        {
            if (obj == this.obj && Started && !Complete)
            {
                OnEndExecute(false);
            }
        }

        protected override void OnUnInstallComplete(PickUpAbleElement arg0)
        {
            
        }
    }
}