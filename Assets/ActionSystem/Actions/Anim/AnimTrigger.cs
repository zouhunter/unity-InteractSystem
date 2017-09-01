using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public class AnimTrigger : ActionTrigger
    {
        private AnimObj[] animObjs;

        private void Awake()
        {
            animObjs = GetComponentsInChildren<AnimObj>(true);
            foreach (var item in animObjs)
            {
                item.StepName = StepName;
            }
        }

        public override IActionCommand CreateCommand()
        {
            foreach (var anim in animObjs){
                anim.RegistEndPlayEvent(OnEndPlayAnim);
            }
            AnimCommand cmd = new AnimCommand(StepName, animObjs);
            return cmd;
        }

        private void OnEndPlayAnim(string StepName)
        {
            if (CurrentStepComplete())
            {
                if (onStepComplete != null)
                    onStepComplete.Invoke(StepName);
            }
            else
            {
                Debug.Log("wait");
            }
        }
        private bool CurrentStepComplete()
        {
            bool complete = true;
            foreach (var item in animObjs)
            {
                complete &= item.Complete;
            }
            return complete;
        }
    }

}