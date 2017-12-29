using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public class MatchCtrl : PlaceController
    {
        public override ControllerType CtrlType { get { return ControllerType.Match; } }

        protected override int PlacePoslayerMask { get { return 1<< Layers.matchPosLayer; } }

        protected override bool CanPlace(PlaceObj matchPos, PickUpAbleElement element, out string why)
        {
            var matchAble = true;
            if (matchPos == null)
            {
                why = "【配制错误】:零件未挂MatchObj脚本";
                Debug.LogError("【配制错误】:零件未挂MatchObj脚本");
                matchAble = false;
            }
            else if (!matchPos.Started)
            {
                matchAble = false;
                why = "操作顺序错误";
            }
            else if (matchPos.AlreadyPlaced)
            {
                matchAble = false;
                why = "已经触发结束";
            }
            else if (matchPos.Name != pickedUpObj.name)
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

        protected override void PlaceObject(PlaceObj pos, PickUpAbleElement pickup)
        {
            pos.Attach(pickedUpObj);
            pickedUpObj.QuickInstall(pos, false, false);
        }

        protected override void PlaceWrong(PickUpAbleElement pickup)
        {
            pickedUpObj.OnPickDown();
        }

    }

}