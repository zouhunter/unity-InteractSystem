using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace InteractSystem.Common.Actions
{
    [NodeGraph. CustomNode("Operate/Match", 10, "InteratSystem")]
    public class MatchNode : PlaceNode<MatchItem>
    {
        public bool completeMoveBack = true;//结束时退回
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Place;
            }
        }
        public bool Matched { get { return obj != null; } }

        public override List<string> NeedElements
        {
            get
            {
                return null;
            }
        }

        protected override void OnBeforeEnd(bool force)
        {
            base.OnBeforeEnd(force);

            if (Matched && completeMoveBack){
                obj.QuickUnInstall();
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
            var obj = GetUnInstalledObj(Name);
            Attach(obj);
            if (Config.quickMoveElement && !ignorePass)
            {
                if (!completeMoveBack)
                {
                    obj.QuickInstall(this,false);
                }
                else
                {
                    OnInstallComplete();
                }
            }
            else
            {
                obj.NormalInstall(this, false);
            }
        }
        /// <summary>
        /// 找出一个没有安装的元素
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public PlaceElement GetUnInstalledObj(string elementName)
        {
            var elements = elementCtrl.GetElements<PlaceElement>(elementName,false);
            if (elements != null)
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    if (!elements[i].HaveBinding)
                    {
                        return elements[i];
                    }
                }
            }
            throw new Exception("配制错误,缺少" + elementName);
        }

        protected override void OnInstallComplete()
        {
            //if (!Completed)
            //{
            //    OnEndExecute(false);
            //}
        }

    }
}