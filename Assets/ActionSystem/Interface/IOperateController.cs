using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

namespace WorldActionSystem
{
    public interface IOperateController
    {
        UnityAction<string> UserError { get; set; }
        ControllerType CtrlType { get; }
        ActionSystem system { get; set; }
        void Update();
    }

}