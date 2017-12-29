using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

namespace WorldActionSystem
{
    public interface IOperateController
    {
        UnityAction<string> userError { get; set; }
        UnityAction<IPlaceItem> onSelect { get; set; }
        ControllerType CtrlType { get; }
        void Update();
    }

}