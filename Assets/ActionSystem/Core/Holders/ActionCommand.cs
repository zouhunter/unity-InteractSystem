using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    [AddComponentMenu(MenuName.ActionCommand)]
    public class ActionCommand : ScriptableObject, IActionCommand, IComparable<ActionCommand>
    {
        [SerializeField, Attributes.DefultName]
        private string _stepName;
        [SerializeField,Attributes. Range(0, 10)]
        private int _queueID;
        [SerializeField, Attributes.Range(0,10)]
        private int _copyCount;
        [SerializeField,Attributes.DefultCameraAttribute()]
        private string _cameraID = CameraController.defultID;
        public string CameraID { get { return _cameraID; } }
        public int QueueID { get { return _queueID; } }
        public int CopyCount { get { return _copyCount; } }
        public string StepName { get { if (string.IsNullOrEmpty(_stepName)) _stepName = name; return _stepName; } }
        public bool Started { get { return _started; } }
        public bool Completed { get { return _completed; } }
        private Events.OperateErrorAction userErr { get; set; }
        private UnityAction<ActionCommand> stepComplete { get; set; }//步骤自动结束方法
        public IActionObj[] ActionObjs { get { return actionObjs; } }
        protected ActionCtroller ActionCtrl { get { return ActionSystem.Instence.actionCtrl; } }
        public ActionObjCtroller ActionObjCtrl { get { return objectCtrl; } }

        protected IActionObj[] actionObjs;
        private ActionObjCtroller objectCtrl;
        //[EnumMask]
        //public ControllerType commandType;

#if ActionSystem_G
        [HideInInspector]
#endif
        public InputField.OnChangeEvent 
            onBeforeActive = new InputField.OnChangeEvent() , 
            onBeforePlayEnd = new InputField.OnChangeEvent(),
            onBeforeUnDo = new InputField.OnChangeEvent();

        protected bool _started;
        protected bool _completed;
        protected ActionGroup _system;
        public ActionGroup system { get { return _system; } set { _system = value; } }
        protected CommandController commandCtrl { get { return system == null ? null : system.CommandCtrl; } }

        protected virtual void OnEnable()
        {
            objectCtrl = new ActionObjCtroller(this);
        }

        public void RegistAsOperate(Events.OperateErrorAction userErr)
        {
            this.userErr = userErr;
        }
        public void RegistComplete(UnityAction<ActionCommand> stepComplete)
        {
            this.stepComplete = stepComplete;
        }
        public int CompareTo(ActionCommand other)
        {
            if (other.QueueID > QueueID)
            {
                return -1;
            }
            else if (other.QueueID == QueueID)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        internal void UserError(string err)
        {
            if (userErr != null)
            {
                userErr.Invoke(StepName, err);
            }
        }

        /// <summary>
        /// 操作过程自动结束
        /// </summary>
        internal bool Complete()
        {
            if (!_completed)
            {
                _started = true;
                _completed = true;
                OnEndExecute();
                if (stepComplete != null) stepComplete.Invoke(this);
                return true;
            }
            else
            {
                Debug.Log("already completed" + name);
                return false;
            }
        }

        public virtual bool StartExecute(bool forceAuto)
        {
            if (!_started)
            {
                _started = true;
                onBeforeActive.Invoke(StepName);
                ActionCtrl.OnStartExecute(objectCtrl, forceAuto);
                return true;
            }
            else
            {
                Debug.Log("already started" + name);
                return false;
            }
        }
        /// <summary>
        /// 强制结束
        /// </summary>
        public virtual bool EndExecute()
        {
            //Debug.Log("EndExecute", gameObject);

            if (!_completed)
            {
                _started = true;
                _completed = true;
                OnEndExecute();
                return true;
            }
            else
            {
                Debug.Log("already completed" + name);
                return false;
            }

        }

        public void OnEndExecute()
        {
            onBeforePlayEnd.Invoke(StepName);
            ActionCtrl.OnEndExecute(objectCtrl);
        }

        public virtual void UnDoExecute()
        {
            _started = false;
            _completed = false;
            onBeforeUnDo.Invoke(StepName);
            ActionCtrl.OnUnDoExecute(objectCtrl);
        }


    }
}

