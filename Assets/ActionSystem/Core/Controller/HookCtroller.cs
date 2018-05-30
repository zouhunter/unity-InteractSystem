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

        protected Graph.OperateNode actionObj { get; set; }
        protected List<int> queueID = new List<int>();
        protected ActionHook[] hooks { get; set; }
        protected bool isForceAuto;

        public HookCtroller(Graph. OperateNode trigger)
        {
            this.actionObj = trigger;
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
                    if(item.Statu == ExecuteStatu.UnStarted)
                    {
                        item.OnStartExecute(isForceAuto);
                    }
                    if (item.Statu != ExecuteStatu.Completed)
                    {
                        item.OnEndExecute(true);
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
                    if (item.Statu != ExecuteStatu.UnStarted)
                    {
                        item.OnUnDoExecute();
                    }
                }
            }
        }

        private void OnCommandObjComplete(ActionHook obj)
        {
            if(!Complete)
            {
                var notComplete = Array.FindAll<ActionHook>(hooks, x => (x as ActionHook).QueueID == obj.QueueID && x.Statu != ExecuteStatu.Completed);
                if (notComplete.Length == 0)
                {
                    if (!ExecuteAStep(isForceAuto))
                    {
                        OnEndExecute();
                        if(actionObj.Statu != ExecuteStatu.Completed)
                        {
                            actionObj.OnEndExecute(false);
                        }
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
                        var obj = item;
                        if (obj.Statu == ExecuteStatu.UnStarted)
                        {
                            obj.onEndExecute = () => OnCommandObjComplete(obj);
                            //Debug.Log("On Execute " + item.name + "of " + id);
                            obj.OnStartExecute(isForceAuto);
                        }
                           
                    }
                }

                return true;
            }
            return false;
        }
    }
}