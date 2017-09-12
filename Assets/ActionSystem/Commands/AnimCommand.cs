using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
namespace WorldActionSystem
{

    public class AnimCommand : IActionCommand
    {
        public AnimObj[] anims;
        public string StepName { get; private set; }

        public int Count { get; private set; }

        public CommandExecute onBeforeExecute;
        public AnimCommand(string stepName, AnimObj[] anims) 
        {
            this.StepName = stepName;
            this.anims = anims;
        }

        public void StartExecute(bool forceAuto)
        {
            if (onBeforeExecute != null) onBeforeExecute.Invoke(StepName);

            foreach (var anim in anims){
                anim.StartExecute();
            }
        }
        public void EndExecute()
        {
            foreach (var anim in anims){
                anim.EndExecute();
            }
        }
        public void UnDoExecute()
        {
            foreach (var anim in anims) {
                anim.UnDoExecute();
            }
        }
    }


}