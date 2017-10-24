using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

using System;
namespace WorldActionSystem
{
    [RequireComponent(typeof(ClickObj))]
    public class ClickHighLighterSender : ActionHightSender
    {
        private ClickObj clickObj;
        protected override void Awake()
        {
            base.Awake();
            clickObj = actionObj as ClickObj;
            clickObj.onMouseEnter.AddListener(OnEnterClickObj);
            clickObj.onMouseExit.AddListener(OnExitClickObj);
        }

        private void OnEnterClickObj()
        {
            SetElementState(true);
        }
        private void OnExitClickObj()
        {
            SetElementState(false);
        }
    }
}