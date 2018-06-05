using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
using NodeGraph.DataModel;
using System;

namespace WorldActionSystem.Graph
{
    public abstract class OperateNode : ActionNode
    {
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    return name;
                }
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        protected ExecuteStatu statu;
        public ExecuteStatu Statu { get { return statu; } }

        [SerializeField, Attributes.DefultCameraAttribute()]
        private string _cameraID;
        public string CameraID { get { return _cameraID; } }
        public UnityAction onEndExecute { get; set; }
        public ActionHook[] Hooks { get { return hooks; } }
        public ActionGroup system { get { return _system; } set { _system = value; } }
        public OperateNode[] StartedList { get { return startedList.ToArray(); } }
        public abstract ControllerType CtrlType { get; }
        public static bool log = false;

        [SerializeField, Attributes.DefultName]
        protected string _name;
        protected bool auto;

        private Hooks.HookCtroller hookCtrl;
        private Binding.ActionBindingCtrl bindingCtrl;
        private Enviroment.EnviromentCtrl enviromentCtrl {
            get {
                return Context.Context.enviromentCtrl;
            }
        }
        private ActionGroup _system;
        protected static List<OperateNode> startedList = new List<OperateNode>();
        [SerializeField]
        private ActionHook[] hooks;//外部结束钩子
        [SerializeField]
        private Binding.ActionBinding[] bindings;
        [SerializeField]
        private Enviroment.EnviromentInfo[] environments;

        protected override void OnEnable()
        {
            base.OnEnable();
            statu = ExecuteStatu.UnStarted;
            InitHookCtrl();
            InitBindingCtrl();
        }
        public override void SetContext(ActionCommand command)
        {
            base.SetContext(command);
            enviromentCtrl.OrignalState(environments);
        }

        private void InitBindingCtrl()
        {
            bindingCtrl = new Binding.ActionBindingCtrl(bindings);
        }
        private void InitHookCtrl()
        {
            hookCtrl = new WorldActionSystem.Hooks.HookCtroller(hooks);
            hookCtrl.onEndExecute += OnHookComplete;
        }

        public override void Initialize(NodeData data)
        {
            base.Initialize(data);
            if (data.InputPoints == null || data.InputPoints.Count == 0)
            {
                data.AddInputPoint("", "actionconnect");
            }
            if (data.OutputPoints == null || data.OutputPoints.Count == 0)
            {
                data.AddOutputPoint("0", "actionconnect", 100);
            }
        }

        public virtual void OnStartExecute(bool auto = false)
        {
            if (log) Debug.Log("OnStartExecute:" + this.Name);
            this.auto = auto;
            if (statu == ExecuteStatu.UnStarted)
            {
                statu = ExecuteStatu.Executing;
                OnStartExecuteInternal(auto);
            }
            else
            {
                Debug.LogError("already started");
            }
        }
        public virtual void OnEndExecute(bool force)
        {
            if (statu != ExecuteStatu.Completed)
            {
                OnBeforeEnd(force);

                if (force)
                {
                    statu = ExecuteStatu.Completed;
                    if (hookCtrl.Statu != ExecuteStatu.Completed)
                    {
                        hookCtrl.OnEndExecute();
                    }
                    CoreEndExecute();
                }
                else
                {
                    if (hookCtrl.Statu == ExecuteStatu.Completed)
                    {
                        statu = ExecuteStatu.Completed;
                        CoreEndExecute();
                        TryCallBack();
                    }
                    else if (hookCtrl.Statu == ExecuteStatu.UnStarted)
                    {
                        hookCtrl.OnStartExecute(auto);
                    }
                    else
                    {
                        Debug.Log("wait:" + Name);
                    }
                }
            }

        }

        private void OnHookComplete()
        {
            if (Statu != ExecuteStatu.Completed)
            {
                statu = ExecuteStatu.Completed;
                CoreEndExecute();
                TryCallBack();
            }
        }
        private void CoreEndExecute()
        {
            enviromentCtrl.CompleteState(environments);
        }
        private void TryCallBack()
        {
            if (onEndExecute != null)
            {
                onEndExecute.Invoke();
            }
        }
        public virtual void OnUnDoExecute()
        {
            //angleCtrl.UnNotice(anglePos);

            if (log) Debug.Log("OnUnDoExecute:" + this);

            if (statu != ExecuteStatu.UnStarted)
            {
                statu = ExecuteStatu.UnStarted;
                if (hookCtrl.Statu != ExecuteStatu.UnStarted){
                    hookCtrl.OnUnDoExecute();
                }
                OnUnDoExecuteInternal();
            }
            else
            {
                Debug.LogError(this + "allready undo");
            }

        }

        protected virtual void OnStartExecuteInternal(bool auto)
        {
            enviromentCtrl.StartState(environments);
            bindingCtrl.OnBeforeActionsStart(this,auto);
            if (!startedList.Contains(this))
            {
                startedList.Add(this);
            }

        }
        protected virtual void OnBeforeEnd(bool force)
        {
            bindingCtrl.OnBeforeActionsPlayEnd(this, force);
            if (startedList.Contains(this))
            {
                startedList.Remove(this);
            }
        }
        protected virtual void OnUnDoExecuteInternal()
        {
            enviromentCtrl.OrignalState(environments);
            bindingCtrl.OnBeforeActionsUnDo(this);
            if (startedList.Contains(this))
            {
                startedList.Remove(this);
            }
        }
    }
}