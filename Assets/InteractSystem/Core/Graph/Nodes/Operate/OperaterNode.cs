using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
using NodeGraph.DataModel;
using System;

namespace InteractSystem.Graph
{
    public abstract class OperaterNode : ActionNode
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
        public UnityAction onEndExecute { get; set; }
        public static bool log = false;

        [SerializeField]
        protected string _name;
        protected bool auto;
        private Hooks.HookCtroller startHookCtrl;
        private Hooks.HookCtroller completeHookCtrl;
        private Binding.OpreaterBindingCtrl bindingCtrl;
        private Enviroment.EnviromentCtrl enviromentCtrl {
            get {
                return Enviroment.EnviromentCtrl.Instence ;
            }
        }
        private ActionGroup _system;
        protected static List<OperaterNode> startedList = new List<OperaterNode>();
        [SerializeField]
        private ActionHook[] startHooks;//外部结束钩子
        [SerializeField]
        private ActionHook[] completeHooks;//外部结束钩子
        [SerializeField]
        private Binding.OperaterBinding[] bindings;
        [SerializeField]
        private Enviroment.EnviromentInfo[] environments;
        protected List<OperateNodeFeature> operateFeatures;
        public List<OperaterNode> StartedList { get { return startedList; } }


        protected override void OnEnable()
        {
            base.OnEnable();
            statu = ExecuteStatu.UnStarted;
            InitHookCtrl();
            InitBindingCtrl();
            operateFeatures = RegistFeatures();
            TryExecuteFeatures(feature => { feature.OnEnable(); });
        }
        public override void SetContext(ActionCommand command)
        {
            base.SetContext(command);
            enviromentCtrl.OrignalState(environments);
        }

        private void InitBindingCtrl()
        {
            bindingCtrl = new Binding.OpreaterBindingCtrl(bindings);
        }
        private void InitHookCtrl()
        {
            startHookCtrl = new Hooks.HookCtroller(startHooks);
            startHookCtrl.onEndExecute += OnStartExecuteInternal;
            completeHookCtrl = new InteractSystem.Hooks.HookCtroller(completeHooks);
            completeHookCtrl.onEndExecute += OnCompleteHooksEnd;
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
                if (startHookCtrl.Statu == ExecuteStatu.Completed)
                {
                    OnStartExecuteInternal();
                }
                else
                {
                    startHookCtrl.OnStartExecute(auto);
                }
            }
            else
            {
                Debug.LogError("already started");
            }
            TryExecuteFeatures(feature => { feature.OnStartExecute(auto); });
        }
        public virtual void OnEndExecute(bool force)
        {
            if (statu != ExecuteStatu.Completed)
            {
                OnBeforeEnd(force);

                if (force)
                {
                    statu = ExecuteStatu.Completed;
                    if (completeHookCtrl.Statu != ExecuteStatu.Completed)
                    {
                        completeHookCtrl.OnEndExecute();
                    }
                    CoreEndExecute();
                }
                else
                {
                    if (completeHookCtrl.Statu == ExecuteStatu.Completed)
                    {
                        statu = ExecuteStatu.Completed;
                        CoreEndExecute();
                        TryCallBack();
                    }
                    else if (completeHookCtrl.Statu == ExecuteStatu.UnStarted)
                    {
                        completeHookCtrl.OnStartExecute(auto);
                    }
                    else
                    {
                        Debug.Log("wait:" + Name);
                    }
                }
            }
            TryExecuteFeatures(feature => { feature.CoreEndExecute(); });

        }
        private void OnCompleteHooksEnd()
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
            if (log) Debug.Log("OnUnDoExecute:" + this);

            if (statu != ExecuteStatu.UnStarted)
            {
                statu = ExecuteStatu.UnStarted;
                if (completeHookCtrl.Statu != ExecuteStatu.UnStarted){
                    completeHookCtrl.OnUnDoExecute();
                }
                OnUnDoExecuteInternal();

                if (startHookCtrl.Statu != ExecuteStatu.UnStarted){
                    startHookCtrl.OnUnDoExecute();
                }
            }
            else
            {
                Debug.LogError(this + "allready undo");
            }
            TryExecuteFeatures(feature => { feature.OnUnDoExecute(); });
        }
        protected virtual void OnStartExecuteInternal()
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
            TryExecuteFeatures(feature => { feature.OnBeforeEnd(force); });
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
        protected virtual List<OperateNodeFeature> RegistFeatures() { return null; }
        public T RetriveFeature<T>() where T : OperateNodeFeature
        {
            if (operateFeatures == null) return default(T);
            else return operateFeatures.Find(x => x is T) as T;
        }
        protected void TryExecuteFeatures(UnityAction<OperateNodeFeature> featureAction)
        {
            if(operateFeatures != null && featureAction != null)
            {
                operateFeatures.ForEach(feature =>
                {
                    featureAction.Invoke(feature);
                });
            }
        }
    }
}