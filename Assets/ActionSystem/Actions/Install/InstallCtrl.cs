using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public class InstallCtrl : PlaceController
    {
        public bool Active { get; private set; }

        protected override int PlacePoslayerMask { get { return 1 << Setting.installPosLayer; } }

        protected override bool CanPlace(PlaceObj placeObj, PickUpAbleElement element, out string why)
        {
            why = null;
            var canplace = true;
            if (placeObj == null)
            {
                Debug.LogError("【配制错误】:零件未挂InstallObj脚本");
            }
            else if (!placeObj.Started)
            {
                canplace = false;
                why = "操作顺序错误";
            }
            else if (placeObj.AlreadyPlaced)
            {
                canplace = false;
                why = "已经安装";
            }
            else if (element.name != placeObj.Name)
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

        protected override void PlaceObject(PlaceObj pos, PickUpAbleElement pickup)
        {
            pos.Attach(pickup);
            pickup.QuickInstall(pos);
            pos.OnEndExecute(false);
        }

        protected override void PlaceWrong(PickUpAbleElement pickup)
        {
            pickedUpObj.NormalUnInstall();
        }
    }

}