using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace InteractSystem.Common.Actions
{
    public class InstallItem : PlaceItem
    {
        public override void PlaceObject(PlaceElement pickup)
        {
            Attach(pickup);
            pickup.QuickInstall(this, true);
            pickup.pickUpAbleItem.PickUpAble = false;
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
            else if (this.element != null)
            {
                canplace = false;
                why = "已经安装";
            }

            else if (elementName != element.Name)
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
            if (Active)
            {
                if (AlreadyPlaced)
                {
                    var obj = Detach();
                    obj.pickUpAbleItem.PickUpAble = true;
                }
                this.element = null;
            }
        }

        public override void OnAutoInstall()
        {
            PlaceElement obj = GetUnInstalledObj(elementName);
            Attach(obj);
            obj.StepActive();
            if (Config.quickMoveElement && !ignorePass)
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
            if (Active)
            {
                OnComplete();
            }
        }
    }
}