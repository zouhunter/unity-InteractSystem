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
        private List<string> HideKeys { get { return GroupHide.HideKeys; } }


        protected override void OnBeforeActive(string step)
        {
            base.OnBeforeActive(step);
            startState = !HideKeys.Contains(key);
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
            if (!HideKeys.Contains(key))
            {
                HideKeys.Add(key);
            }
        }

        private void Remove()
        {
            if (string.IsNullOrEmpty(key)) return;

            if (HideKeys.Contains(key))
            {
                HideKeys.Remove(key);
            }
        }

    }
}
