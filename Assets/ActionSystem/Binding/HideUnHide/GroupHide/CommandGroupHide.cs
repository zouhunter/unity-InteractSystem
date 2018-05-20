using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem.Binding
{
    public class CommandGroupHide : ActionCommandBinding
    { 
        [SerializeField]
        private string key;
        [SerializeField]
        private bool startActive;
        [SerializeField]
        private bool completeActive;
        private bool startState;

        protected override void OnBeforeActive(string step)
        {
            base.OnBeforeActive(step);
            startState = !GroupHide.Contains(key);
            if (!startActive)
            {
                Record();
            }
            else
            {
                Remove();
            }
        }
        protected override void OnBeforeUnDo(string step)
        {
            base.OnBeforeUnDo(step);
            if(startState)
            {
                Remove();
            }
            else
            {
                Record();
            }
        }
        protected override void OnBeforePlayEnd(string step)
        {
            base.OnBeforePlayEnd(step);
            if (!completeActive)
            {
                Record();
            }
            else
            {
                Remove();
            }
        }

        private void Record()
        {
            if (string.IsNullOrEmpty(key)) return;
            if (!GroupHide.Contains(key))
            {
                GroupHide.Record(key);
            }
        }

        private void Remove()
        {
            if (string.IsNullOrEmpty(key)) return;

            if (GroupHide.Contains(key))
            {
                GroupHide.Remove(key);
            }
        }

    }
}
