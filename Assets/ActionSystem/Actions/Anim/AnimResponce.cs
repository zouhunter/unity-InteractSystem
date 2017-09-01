using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public class AnimResponce : ActionResponce
    {
        private AnimObj[] animObjs;

        private void Awake()
        {
            animObjs = GetComponentsInChildren<AnimObj>(true);
        }

        public override IActionCommand CreateCommand()
        {
            foreach (var anim in animObjs)
            {
                anim.RegistEndPlayEvent(OnEndPlayAnim);
            }
            AnimCommand cmd = new AnimCommand(StepName, animObjs);
            return cmd;
        }

        private void OnEndPlayAnim(string StepName)
        {
            if (CurrentStepComplete())
            {
                if (OnStepEnd != null)
                    OnStepEnd.Invoke(StepName);
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