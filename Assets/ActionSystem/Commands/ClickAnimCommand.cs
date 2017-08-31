﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class ClickAnimCommand : ActionCommand
    {
        private AnimGroup animParent;
        private BtnGroup btnParent;

        public ClickAnimCommand(string stapName, BtnGroup btnParent, AnimGroup animParent) : base(stapName)
        {
            this.btnParent = btnParent;
            this.animParent = animParent;
            btnParent.onAllButtonClicked = () => animParent.PlayAnim(StepName);
        }
        public override void StartExecute(bool forceAuto)
        {
            if (forceAuto) btnParent.SetAllButtonClicked(StepName,true);
            else
            {
                btnParent.SetButtonClickAbleQueue(StepName);
            }
            base.StartExecute(forceAuto);
        }
        public override void EndExecute()
        {
            btnParent.SetAllButtonClicked(StepName,false);
            animParent.SetAnimEnd(StepName);
            base.EndExecute();
        }
        public override void UnDoCommand()
        {
            btnParent.SetAllButtonUnClickAble(StepName);
            btnParent.SetButtonNotClicked(StepName);
            animParent.SetAnimUnPlayed(StepName);
            base.UnDoCommand();
        }
    }

}