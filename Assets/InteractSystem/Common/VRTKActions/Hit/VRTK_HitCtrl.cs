using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractSystem;
using VRTK;
namespace InteractSystem.VRTKActions
{
    public class VRTK_HitCtrl : MonoBehaviour
    {
        private VRTK_HitItem hitObj;
        private GameObject lastSelected;

        void Update()
        {
            VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(gameObject);
            //Only continue if the controller reference is valid
            if (!VRTK_ControllerReference.IsValid(controllerReference))
            {
                return;
            }

            Vector2 currentTriggerAxis = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Trigger, controllerReference);
            Vector2 currentGripAxis = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Grip, controllerReference);
            Vector2 currentTouchpadAxis = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Touchpad, controllerReference);

            // Trigger Pressed end
            if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
            {
              
            }
        }
         
}
}