using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Actions
{
    public class InstallItem : PlaceItem
    {
        public override void PlaceObject(PlaceElement pickup)
        {
            Attach(pickup);
            pickup.QuickInstall(this);
            pickup.PickUpAble = false;
        }

        public override bool CanPlace(PlaceElement element, out string why)
        {
            why = null;
            var canplace = true;
            if (!this.Actived)
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
        protected override void OnSetInActive(UnityEngine.Object target)
        {
            base.OnSetInActive(target);
            if (!AlreadyPlaced)
            {
                PlaceElement obj = GetUnInstalledObj(contentFeature.ElementName,true);
                obj.QuickInstall(this);
                obj.SetInActive(this);
            }
        }

        protected override void OnUnInstallComplete()
        {
            contentFeature.Element.RemovePlayer(this);
            contentFeature.Element.UnDoChanges(this);
 
            if (Actived)
            {
                if (AlreadyPlaced)
                {
                    var obj = Detach();
                    obj.PickUpAble = true;
                }
                contentFeature.Element = null;
            }
        }

        public override void OnAutoExecute(UnityEngine.Object node)
        {
            PlaceElement obj = GetUnInstalledObj(contentFeature.ElementName,true);

            if (Config.Instence.quickMoveElement && !ignorePass)
            {
                obj.QuickInstall(this);
            }
            else
            {
                obj.NormalInstall(this);
            }
        }

        protected override void OnInstallComplete()
        {
            contentFeature.Element.RecordPlayer(this);
            contentFeature.Element.SetInActive(this);
            if (Actived)
            {
                Debug.Log(firstLock);
                completeFeature.OnComplete(firstLock);
            }
            else
            {
               if(log) Debug.Log(this + " in active!");
            }
        }
    }
}