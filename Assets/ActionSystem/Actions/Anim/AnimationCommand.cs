using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class AnimationCommand : ActionCommand
    {
        protected override void Awake()
        {
            base.Awake();
            foreach (AnimObj anim in  ActionObjs){
                anim.RegistAutoEndPlayEvent(OnEndPlayAnim);
            }
        }
        private void OnEndPlayAnim()
        {
            if (CurrentStepComplete())
            {
                Complete();
            }
            else
            {
                Debug.Log("wait");
            }
        }
        protected bool CurrentStepComplete()
        {
            bool complete = true;
            foreach (var item in actionObjs){
                complete &= item.Complete;
            }
            return complete;
        }

    }

}