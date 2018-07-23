using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Actions
{
    public class MatchItem : PlaceItem
    {
        [SerializeField,Attributes.CustomField("结束重置目标元素")]
        protected bool completeMoveBack;

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
            else if (contentFeature.Element != null)
            {
                matchAble = false;
                why = "已经触发结束";
            }
            else if (contentFeature.ElementName != element.Name)
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
        
        public bool Matched { get { return contentFeature.Element != null; } }


        public override void StepComplete()
        {
            base.StepComplete();
            if (!AlreadyPlaced)
            {
                PlaceElement obj = GetUnInstalledObj(contentFeature.ElementName);
                Attach(obj);
                obj.QuickInstall(this, false);
                obj.StepComplete();
            }
            if (Matched && completeMoveBack)
            {
                (contentFeature.Element as PlaceElement).QuickUnInstall();
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
            base.OnInstallComplete();
            if (Active)
            {
                completeFeature. OnComplete();
            }
        }

        public override void AutoExecute(Graph.OperaterNode operateNode)
        {
            var obj = GetUnInstalledObj(contentFeature.ElementName);
            Attach(obj);
            if (Config.Instence.quickMoveElement && !ignorePass)
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