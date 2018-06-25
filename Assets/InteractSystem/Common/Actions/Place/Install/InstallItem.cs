using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace InteractSystem.Actions
{
    public class InstallItem : PlaceItem
    {
        public override void PlaceObject(PlaceElement pickup)
        {
            Attach(pickup);
            pickup.QuickInstall(this, true);
            pickup.PickUpAble = false;
        }

        public override bool CanPlace(PlaceElement element, out string why)
        {
            why = null;
            var canplace = true;
            if (!this.Active)
            {
                canplace = false;
                why = "操作顺序错误";
            }
            else if (contentFeature.Element != null)
            {
                canplace = false;
                why = "已经安装";
            }

            else if (contentFeature.ElementName != element.Name)
            {
                canplace = false;
                why = "零件不匹配";
            }
            else
            {
                canplace = true;
            }
            return canplace;
        }

        protected override void OnUnInstallComplete()
        {
            base.OnUnInstallComplete();
            if (Active)
            {
                if (AlreadyPlaced)
                {
                    var obj = Detach();
                    obj.PickUpAble = true;
                }
                contentFeature.Element = null;
            }
        }

        public override void AutoExecute(Graph.OperaterNode node)
        {
            PlaceElement obj = GetUnInstalledObj(contentFeature.ElementName);
            Attach(obj);
            obj.StepActive();
            if (Config.Instence.quickMoveElement && !ignorePass)
            {
                obj.QuickInstall(this, true);
            }
            else
            {
                obj.NormalInstall(this, true);
            }
        }

        protected override void OnInstallComplete()
        {
            base.OnInstallComplete();
            if (Active)
            {
               completeFeature.OnComplete();
            }
            else
            {
                Debug.Log(this + " in active!");
            }
        }
    }
}