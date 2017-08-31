using System;
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
            btnParent.onAllButtonClicked = () => animParent.PlayAnim(StapName);
        }
        public override void StartExecute(bool forceAuto)
        {
            btnParent.SetButtonClickAbleQueue(StapName);
            if (forceAuto) btnParent.SetButtonClicked(StapName);
            base.StartExecute(forceAuto);
        }
        public override void EndExecute()
        {
            btnParent.SetButtonClicked(StapName);
            animParent.SetAnimEnd(StapName);
            base.EndExecute();
        }
        public override void UnDoCommand()
        {
            btnParent.SetAllButtonUnClickAble(StapName);
            btnParent.SetButtonNotClicked(StapName);
            animParent.SetAnimUnPlayed(StapName);
            base.UnDoCommand();
        }
    }

}