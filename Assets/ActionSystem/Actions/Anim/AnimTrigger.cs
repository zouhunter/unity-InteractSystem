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

        public override IList<IActionCommand> CreateCommands()
        {
            foreach (AnimObj anim in actionObjs){
                anim.RegistAutoEndPlayEvent(OnEndPlayAnim);
            }
            var cmds = new List<IActionCommand>();
            for (int i = 0; i < repeat; i++)
            {
                cmds.Add(new AnimCommand(StepName,repeat,Array.ConvertAll<ActionObj,AnimObj>(actionObjs,x=>(AnimObj)x)));
            }
            return cmds;
        }

        private void OnEndPlayAnim(string StepName)
        {
            if (CurrentStepComplete())
            {
                OnComplete();
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