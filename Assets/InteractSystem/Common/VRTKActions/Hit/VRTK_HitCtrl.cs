using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractSystem;
namespace InteractSystem.VRTKActions
{
public class VRTK_HitCtrl :OperateController
{
        private VRTK_HitItem hitObj;
        private GameObject lastSelected;
		public override ControllerType CtrlType { get{return ControllerType.VR_Hit;} }
		
		public override void Update(){
			
		}
}    
}