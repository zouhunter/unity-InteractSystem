﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class RotateCommand : IActionCommand
    {
        private RotateTrigger rotParent;
        public string StepName { get; private set; }

        public RotateCommand(string stepName,int count, RotateTrigger rotParent) 
        {
            this.StepName = stepName;
            this.rotParent = rotParent;
        }
        public  void StartExecute(bool forceAuto)
        {
            rotParent.ActiveStep(StepName);
            if (forceAuto) {
                rotParent.SetRotateComplete(true);
            }
            else{
                rotParent.SetRotateQueue(StepName);
            }
        }
        public  void EndExecute()
        {
            rotParent.SetRotateComplete();
        }
        public  void UnDoExecute()
        {
            rotParent.SetStepUnDo(StepName);
        }
    }

}
