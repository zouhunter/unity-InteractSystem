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
        public override IActionCommand CreateCommand()
        {
            foreach (AnimObj anim in actionObjs){
                anim.RegistAutoEndPlayEvent(OnEndPlayAnim);
            }
            AnimCommand cmd = new AnimCommand(StepName,Array.ConvertAll<ActionObj,AnimObj>(actionObjs,x=>(AnimObj)x));
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
                //Debug.Log("wait");
            }
        }
        private bool CurrentStepComplete()
        {
            bool complete = true;
            foreach (AnimObj item in actionObjs)
            {
                complete &= item.Complete;
            }
            return complete;
        }
    }

}