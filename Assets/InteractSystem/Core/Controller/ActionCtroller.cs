using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace InteractSystem
{
    public class ActionCtroller
    {
        public ActionCommand activeCommand { get; private set; }
        public Structure.ActionStateMechine stateMechine { get { return activeCommand == null ? null : activeCommand.objectCtrl; } }
        public static bool log = false;
        public UnityAction<Graph.OperaterNode> onActionStart;
        private CameraController cameraCtrl
        {
            get
            {
                return CameraController.Instence;
            }
        }
        private PickUpController pickupCtrl { get { return PickUpController.Instence; } }
        private static ActionCtroller _instence;
        public static ActionCtroller Instence
        {
            get
            {
                if (_instence == null)
                {
                    _instence = new ActionCtroller(ActionSystem.Instence);
                }
                return _instence;
            }
        }

        public ActionCtroller(MonoBehaviour holder)
        {
            pickupCtrl.onPickup += (OnPickUpObj);
        }

        /// 激活首要对象
        /// </summary>
        /// <param name="obj"></param>
        internal void OnPickUpObj(PickUpAbleComponent obj)
        {
            var actionItems = obj.GetComponentsInChildren<ActionItem>();
            if (actionItems != null && actionItems.Length > 0)
            {
                ElementController.Instence.SetPriority(actionItems);
            }
        }

        public virtual void SetContext(ActionCommand activeCommand)
        {
            this.activeCommand = activeCommand;
        }

        public virtual void OnStartExecute(bool forceAuto)
        {
            if (stateMechine != null)
            {
                stateMechine.OnStartExecute(forceAuto);
            }
        }

        public virtual void OnEndExecute()
        {
            if (stateMechine != null)
            {
                stateMechine.OnEndExecute(true);
            }
        }

        public virtual void OnUnDoExecute()
        {
            if (stateMechine != null)
            {
                stateMechine.OnUnDoExecute(true);
            }
        }

        public virtual void OnEndExecuteStarted()
        {
            if (stateMechine != null)
            {
                stateMechine.OnEndExecute(false);
            }
        }

        public virtual void OnUnDoExecuteOne()
        {
            if (stateMechine != null)
            {
                stateMechine.OnUnDoExecute(false);
            }
        }
    }
}