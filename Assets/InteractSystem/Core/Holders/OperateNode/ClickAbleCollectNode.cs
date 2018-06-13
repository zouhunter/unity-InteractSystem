using UnityEngine;
using System.Collections.Generic;
using InteractSystem.Graph;
using System;
using System.Linq;

namespace InteractSystem
{
    /// <summary>
    ///可点击操作的节点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ClickAbleCollectNode<T> : CompleteAbleCollectNode<ClickAbleCompleteAbleActionItem> ,IRuntimeCtrl where T:ClickAbleCompleteAbleActionItem
    {
        public abstract ControllerType CtrlType { get; }
    }
}
