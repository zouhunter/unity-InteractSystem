using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace InteractSystem.Common.Actions
{
    [AddComponentMenu(MenuName.MatchObj)]
    public class MatchObj : PlaceObj
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

        public override void PlaceObject(PlaceElement pickup)
        {
            Attach(pickup);
            pickup.QuickInstall(this, false);
        }

        public override bool CanPlace(PickUpAbleItem element, out string why)
        {
            var matchAble = true;
            if (this == null)
            {
                why = "【配制错误】:零件未挂MatchObj脚本";
                Debug.LogError("【配制错误】:零件未挂MatchObj脚本");
                matchAble = false;
            }
            //else if (!this.Started)
            //{
            //    matchAble = false;
            //    why = "操作顺序错误";
            //}
            else if (this.AlreadyPlaced)
            {
                matchAble = false;
                why = "已经触发结束";
            }
            else if (this.Name != element.Name)
            {
                matchAble = false;
                why = "零件不匹配";
            }
            else
            {
                why = null;
                matchAble = true;
            }
            return matchAble;
        }

    }
}