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
        public bool Matched { get { return obj != null; } }
        public PickUpAbleElement obj { get; private set; }

        void Awake()
        {
            gameObject.layer = Setting.matchPosLayer;
            ElementController.onInstall += OnInstallComplete;
        }

        private void OnDestroy()
        {
            ElementController.onInstall -= OnInstallComplete;
        }

        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            if (auto || autoMatch)//查找安装点并安装后结束
            {
                PickUpAbleElement obj = ElementController.GetUnInstalledObj(name);

                Attach(obj);

                if (Setting.ignoreMatch)
                {
                    obj.QuickMoveTo(gameObject);
                }
                else
                {
                    obj.NormalMoveTo(gameObject);
                }
            }
        }
        public override void OnEndExecute(bool force)
        {
            base.OnEndExecute(force);
            if (!Matched)
            {
                PickUpAbleElement obj = ElementController.GetUnInstalledObj(name);
                Attach(obj);
            }
        }
        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            if (Matched){
                Detach();
            }
        }
        private void OnInstallComplete(PickUpAbleElement obj)
        {
            if (obj == this.obj)
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