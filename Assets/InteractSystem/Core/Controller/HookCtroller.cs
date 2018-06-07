using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace InteractSystem.Hooks
{
    public class HookCtroller 
    {
        protected ExecuteStatu statu;
        public ExecuteStatu Statu { get { return statu; } }
        protected List<int> queueID = new List<int>();
        protected ActionHook[] hooks { get; set; }
        protected bool isForceAuto;
        public UnityEngine.Events.UnityAction onEndExecute { get; set; }
        protected bool active { get;private set; }

        public HookCtroller(params ActionHook[] actionHooks)
        {
            if(actionHooks != null)//删除无用的hooks
            {
                 actionHooks = (from hook in actionHooks
                                 where hook != null
                                 select hook).ToArray();
            }
          
            if (actionHooks != null && actionHooks.Length > 0)
            {
                active = true;
                statu = ExecuteStatu.UnStarted;
                hooks = new ActionHook[actionHooks.Length];
                for (int i = 0; i < actionHooks.Length; i++)
                {
                    hooks[i] = ScriptableObject.Instantiate(actionHooks[i]);
                }
            }
            else
            {
                statu = ExecuteStatu.Completed;
                active = false;
            }
        }

        public virtual void OnStartExecute(bool forceAuto)
        {
            if (!active) return;

            if (statu == ExecuteStatu.UnStarted)
            {
                statu = ExecuteStatu.Executing;
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
            if (!active) return;

            if (statu != ExecuteStatu.Completed)
            {
                statu = ExecuteStatu.Completed;
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
            if (!active) return;

            if (statu != ExecuteStatu.UnStarted)
            {
                statu = ExecuteStatu.UnStarted;
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
            if(statu != ExecuteStatu.Completed)
            {
                var notComplete = Array.FindAll<ActionHook>(hooks, x => (x as ActionHook).QueueID == obj.QueueID && x.Statu != ExecuteStatu.Completed);
                if (notComplete.Length == 0)
                {
                    if (!ExecuteAStep(isForceAuto))
                    {
                        OnEndExecute();
                        if(onEndExecute != null)
                        {
                            onEndExecute.Invoke();
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