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
            //Attach(pickup);
            //pickup.QuickInstall(this, true);
            //pickup.PickUpAble = false;
        }

        public override bool CanPlace(PickUpAbleItem element, out string why)
        {
            why = null;
            return false;

            //why = null;
            //var canplace = true;
            //if (!this.Started)
            //{
            //    canplace = false;
            //    why = "操作顺序错误";
            //}
            //else if (this.AlreadyPlaced)
            //{
            //    canplace = false;
            //    why = "已经安装";
            //}

            //else if (element.Name != this.Name)
            //{
            //    canplace = false;
            //    why = "零件不匹配";
            //}
            //else
            //{
            //    canplace = true;
            //}
            //return canplace;
        }
    }
}