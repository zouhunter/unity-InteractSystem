using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Internal;

namespace InteractSystem.Common.Actions
{
    /// <summary>
    /// 执行完指定顺序的InstallItem
    /// </summary>
    [NodeGraph.CustomNode("Operate/Install", 10, "InteratSystem")]
    public class InstallNode : PlaceNode
    {
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Place;
            }
        }

        /// <summary>
        /// 自动进行安装演示
        /// </summary>
        protected override void AutoCompleteItems()
        {
           //
        }
    }
}