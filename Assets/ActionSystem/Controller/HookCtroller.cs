using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class HookCtroller 
    {
        protected bool _complete;
        public bool Complete { get { return _complete; } }
        protected bool _started;
        public bool Started { get { return _started; } }

        protected ActionObj trigger { get; set; }
        protected List<int> queueID = new List<int>();
        protected ActionHook[] hooks { get; set; }
        protected bool isForceAuto;
        public HookCtroller(ActionObj trigger)
        {
            this.trigger = trigger;
            hooks = trigger.Hooks;
        }

        public virtual void OnStartExecute(bool forceAuto)
        {
            if(!_started)
            {
                _started = true;
                _complete = false;
                this.isForceAuto = forceAuto;
                ChargeQueueIDs();
                ExecuteAStep(isForceAuto);
            }
        }

        private void ChargeQueueIDs()
        {
            queueID.Clear();
            foreach (ActionHook item in hooks)
            {
                if (!queueID.Contains(item.QueueID))
                {
                    queueID.Add(item.QueueID);
                }
            }
            queueID.Sort();
        }

        public virtual void OnEndExecute()
        {
            if (!_complete)
            {
                _complete = true;
                _started = true;
                foreach (var item in hooks)
                {
                    if (!item.Complete)
                    {
                        item.OnEndExecute();
                    }
                }
            }
        }

        public virtual void OnUnDoExecute()
        {
            if (_started)
            {
                _started = false;
                _complete = false;
                foreach (var item in hooks)
                {
                    if (item.Started)
                    {
                        item.OnUnDoExecute();
                    }
                }
            }
        }

        private void OnCommandObjComplete(int id)
        {
            if(!Complete)
            {
                var notComplete = Array.FindAll<ActionHook>(hooks, x => (x as ActionHook).QueueID == id && !x.Complete);
                if (notComplete.Length == 0)
                {
                    if (!ExecuteAStep(isForceAuto))
                    {
                        OnEndExecute();
                        trigger.OnEndExecute();
                    }
                }
            }
           
        }

        protected bool ExecuteAStep(bool auto)
        {
            if (queueID.Count > 0)
            {
                var id = queueID[0];
                queueID.RemoveAt(0);
                var neetActive = Array.FindAll<ActionHook>(hooks, x => (x as ActionHook).QueueID == id);
                if (neetActive.Length > 0)
                {
                    foreach (ActionHook item in neetActive)
                    {
                        item.onEndExecute = OnCommandObjComplete;
                        Debug.Log("On Execute " + item.name + "of " + id);
                        item.OnStartExecute(isForceAuto);
                    }
                }

                return true;
            }
            return false;
        }
    }
}