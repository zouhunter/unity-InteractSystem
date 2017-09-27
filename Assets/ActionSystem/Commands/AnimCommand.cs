//using System;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using System.Collections;
//using System.Collections.Generic;
//namespace WorldActionSystem
//{
//    [System.Serializable]
//    public class AnimationCommand : IActionCommand
//    {
//        public string StepName { get; private set; }
//        private ActionCommand trigger { get; set; }

//        public void InitCommand(string stepName,ActionCommand trigger)
//        {
//            this.StepName = stepName;
//            this.trigger = trigger;
//            foreach (AnimObj anim in trigger.ActionObjs)
//            {
//                anim.RegistAutoEndPlayEvent(OnEndPlayAnim);
//            }
//        }
      
//        public void StartExecute(bool forceAuto)
//        {
//            foreach (AnimObj anim in trigger.ActionObjs)
//            {
//                anim.OnStartExecute();
//            }
//        }
//        public void EndExecute()
//        {
//            foreach (AnimObj anim in trigger.ActionObjs)
//            {
//                anim.OnEndExecute();
//            }
//        }
//        public void UnDoExecute()
//        {
//            foreach (AnimObj anim in trigger.ActionObjs)
//            {
//                anim.OnUnDoExecute();
//            }
//        }
//        private void OnEndPlayAnim(string StepName)
//        {
//            if (CurrentStepComplete())
//            {
//                trigger.Complete();
//            }
//            else
//            {
//                Debug.Log("wait");
//            }
//        }
//        protected bool CurrentStepComplete()
//        {
//            bool complete = true;
//            foreach (var item in trigger.ActionObjs)
//            {
//                complete &= item.Complete;
//            }
//            return complete;
//        }
//    }

//}