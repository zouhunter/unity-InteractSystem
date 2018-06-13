using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Common.Actions
{
    public class MatchItem : PlaceItem
    {
        public bool completeMoveBack;

        public override void PlaceObject(PlaceElement pickup)
        {
            Attach(pickup);
            pickup.QuickInstall(this, false);
        }

     
        public override bool CanPlace(PlaceElement element, out string why)
        {
            var matchAble = true;
            if (this == null)
            {
                why = "【配制错误】:零件未挂MatchObj脚本";
                Debug.LogError("【配制错误】:零件未挂MatchObj脚本");
                matchAble = false;
            }
            else if (!this.Active)
            {
                matchAble = false;
                why = "操作顺序错误";
            }
            else if (this.element != null)
            {
                matchAble = false;
                why = "已经触发结束";
            }
            else if (this.elementName != element.Name)
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
        
        public bool Matched { get { return element != null; } }

        public override void StepComplete()
        {
            base.StepComplete();
            if (Matched && completeMoveBack)
            {
                element.QuickUnInstall();
            }
        }

        public override void StepUnDo()
        {
            base.StepUnDo();

            if (Matched)
            {
                var obj = Detach();
                obj.QuickUnInstall();
            }
        }

        protected override void OnInstallComplete()
        {
            if (Active)
            {
                OnComplete();
            }
        }

        public override void AutoExecute()
        {
            var obj = GetUnInstalledObj(elementName);
            Attach(obj);
            if (Config.quickMoveElement && !ignorePass)
            {
                if (!completeMoveBack)
                {
                    obj.QuickInstall(this, false);
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
    }
}