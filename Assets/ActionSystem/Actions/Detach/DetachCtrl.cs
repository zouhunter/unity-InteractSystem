using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WorldActionSystem
{
    public class DetachCtrl : OperateController
    {
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Detach;
            }
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}