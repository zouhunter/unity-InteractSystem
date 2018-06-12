using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Common.Actions
{
  
    public class ClickItem : ClickAbleActionItem
    {
        [SerializeField]
        protected int clickableCount = 1;
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
    }
}