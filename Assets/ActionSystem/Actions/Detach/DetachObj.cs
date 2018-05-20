using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WorldActionSystem
{
    public class DetachObj : ActionObj
    {
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Detach;
            }
        }
    }
}
