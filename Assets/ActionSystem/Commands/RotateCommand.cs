using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class RotateCommand : ActionCommand
    {
        private AnimGroup animParent;
        private RotGroup rotParent;


        public RotateCommand(string stapName, RotGroup rotParent, AnimGroup animParent) : base(stapName)
        {
            this.rotParent = rotParent;
            this.animParent = animParent;
        }
        public override void StartExecute(bool forceAuto)
        {
            rotParent.ActiveStep(StepName);
            if (forceAuto) {
                rotParent.SetRotateComplete(true);
            }
            else{
                rotParent.SetRotateQueue(StepName);
            }
            base.StartExecute(forceAuto);
        }
        public override void EndExecute()
        {
            rotParent.SetRotateComplete();
            animParent.SetAnimEnd(StepName);
            base.EndExecute();
        }
        public override void UnDoCommand()
        {
            rotParent.SetRotateStart(StepName);
            animParent.SetAnimUnPlayed(StepName);
            base.UnDoCommand();
        }
    }

}
