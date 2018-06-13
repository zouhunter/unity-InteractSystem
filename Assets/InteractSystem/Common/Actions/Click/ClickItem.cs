using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Common.Actions
{
  
    public class ClickItem : ClickAbleCompleteAbleActionItem
    {
        [SerializeField]
        protected int clickableCount = 1;
        [SerializeField]
        protected float autoCompleteTime = 2;
        protected CoroutineController coroutineCtrl { get {return CoroutineController.Instence; } }

        public override bool OperateAble
        {
            get { return clickableCount > targets.Count; }
        }

        protected override string LayerName
        {
            get
            {
                return Layers.clickItemLayer;
            }
        }
        public override void AutoExecute()
        {
            coroutineCtrl.DelyExecute(OnComplete, autoCompleteTime);
        }
    }
}