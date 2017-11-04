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
    public class MatchObj : ActionObj
    {
        public bool autoMatch;
        public bool ignorePass;
        public Transform passBy;
        public bool straightMove;//直线移动
        public bool ignoreMiddle;//忽略中间点
        public bool completeMoveBack = true;//结束时退回
        public bool Matched { get { return obj != null; } }
        public PickUpAbleElement obj { get; private set; }

        void Awake()
        {
            gameObject.layer = Setting.matchPosLayer;
        }
        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            ElementController.onInstall += OnInstallComplete;
            PickUpAbleElement obj = ElementController.GetUnInstalledObj(Name);
            SetStartNotify(obj);
            if (auto || autoMatch)//查找安装点并安装后结束
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
        }
        private void SetStartNotify(PickUpAbleElement obj)
        {
            if (!obj.Installed)
            {
                obj.StepActive();
            }
        }
        private void SetCompleteNotify(PickUpAbleElement obj,bool undo)
        {
            if (obj.Installed) return;
            
            if (undo)
            {
                obj.StepUnDo();
            }
            else
            {
                obj.StepComplete();
            }
        }

        public override void OnEndExecute(bool force)
        {
            ElementController.onInstall -= OnInstallComplete;
            base.OnEndExecute(force);
            if (!Matched){
                PickUpAbleElement obj = ElementController.GetUnInstalledObj(Name);
                Attach(obj);
            }
            if (completeMoveBack){
                obj.QuickMoveBack();
            }
            SetCompleteNotify(obj, false);
        }
        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            if (Matched){
               var obj = Detach();
                obj.QuickMoveBack();
                SetCompleteNotify(obj, true);
            }
        }
        private void OnInstallComplete(PickUpAbleElement obj)
        {
            if (obj == this.obj && Started && !Complete)
            {
               OnEndExecute(false);
            }
        }
        public bool Attach(PickUpAbleElement obj)
        {
            if (this.obj != null){
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
            obj = default(PickUpAbleElement);
            return old;
        }
    }
}