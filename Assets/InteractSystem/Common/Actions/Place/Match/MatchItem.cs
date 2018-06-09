using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace InteractSystem.Common.Actions
{
    public class MatchItem : PlaceItem
    {

        public override void PlaceObject(PlaceElement pickup)
        {
            //Attach(pickup);
            //pickup.QuickInstall(this, false);
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
            //else if (this.AlreadyPlaced)
            //{
            //    matchAble = false;
            //    why = "已经触发结束";
            //}
            //else if (this.Name != element.Name)
            //{
            //    matchAble = false;
            //    why = "零件不匹配";
            //}
            else
            {
                why = null;
                matchAble = true;
            }
            return matchAble;
        }
    }
}