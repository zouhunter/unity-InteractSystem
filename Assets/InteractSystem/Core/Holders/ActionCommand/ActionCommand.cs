using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace InteractSystem
{
    [AddComponentMenu(MenuName.ActionCommand)]
    public class ActionCommand : NodeGraph.DataModel.NodeGraphObj
    {
        //步骤名
        [SerializeField, Attributes.DefultName]
        public string _stepName;
        [SerializeField]//功能绑定
        protected Binding.CommandBinding[] commandBindings;
        [SerializeField]//环境对象
        private Enviroment.EnviromentInfo[] environments;
        [SerializeField]//开始等待
        private ActionHook[] startHooks;
        [SerializeField]//结束等待
        private ActionHook[] completeHooks;
        protected UnityAction<string, int, int> onActionObjStartExecute { get; set; }
        public string StepName { get { if (string.IsNullOrEmpty(_stepName)) _stepName = name; return _stepName; } }
        public ExecuteStatu Statu { get { return statu; } }
        private Events.OperateErrorAction userErr { get; set; }
        private UnityAction<string> stepComplete { get; set; }//步骤自动结束方法
        protected ActionCtroller actionCtrl { get { return ActionCtroller.Instence; } }
        public Transform Context { get; private set; }//上下文

        //执行状态
        protected ExecuteStatu statu = ExecuteStatu.UnStarted;
        //步骤控制器
        public Structure.ActionStateMechine objectCtrl { get; private set; }
        //hook控制器
        protected Hooks.HookCtroller startHookCtrl;
        protected Hooks.HookCtroller completeHookCtrl;
        protected Binding.CommandBingCtrl commandBindingCtrl;
        protected Enviroment.EnviromentCtrl enviromentCtrl { get { return Enviroment.EnviromentCtrl.Instence; } }
        protected bool forceAuto;

        protected virtual void OnEnable()
        {
            statu = ExecuteStatu.UnStarted;
            InitHookCtrl();
            InitBindgCtrl();
            InitOperateCtrl();
        }

        private void InitOperateCtrl()
        {
            objectCtrl = new Structure.ActionStateMechine(this);
            objectCtrl.onComplete = OnEndExecute;
        }

        private void InitBindgCtrl()
        {
            if(commandBindings != null && commandBindings.Length > 0)
            {
                commandBindingCtrl = new Binding.CommandBingCtrl(commandBindings);
            }
        }

        private void InitHookCtrl()
        {
            startHookCtrl = new Hooks.HookCtroller(startHooks);
            startHookCtrl.onEndExecute += StartExecuteInternal;
            completeHookCtrl = new InteractSystem.Hooks.HookCtroller(completeHooks);
            completeHookCtrl.onEndExecute += OnCompleteHookEnd;
        }

        public void SetContext(Transform group)
        {
            //重置当前command的信息
            this.Context = group;
            enviromentCtrl.OrignalState(environments);
        }

        public void RegistAsOperate(Events.OperateErrorAction userErr)
        {
            this.userErr = userErr;
        }

        public void RegistComplete(UnityAction<string> stepComplete)
        {
            this.stepComplete = stepComplete;
        }
    
        internal void UserError(string err)
        {
            if (userErr != null)
            {
                userErr.Invoke(StepName, err);
            }
        }

        public virtual bool StartExecute(bool forceAuto)
        {
            this.forceAuto = forceAuto;
            if (statu == ExecuteStatu.UnStarted)
            {
                statu = ExecuteStatu.Executing;
                if (startHookCtrl.Statu == ExecuteStatu.Completed)
                {
                    StartExecuteInternal();
                }
                else
                {
                    startHookCtrl.OnStartExecute(forceAuto);
                }
                return true;
            }
            else
            {
                Debug.Log("already started" + name);
                return false;
            }
        }

        private void StartExecuteInternal()
        {
            OnBeforeActionsStart();
            actionCtrl.SetContext(this);
            actionCtrl.OnStartExecute(forceAuto);
        }

        private void OnCompleteHookEnd()
        {
            if(statu != ExecuteStatu.Completed)
            {
                statu = ExecuteStatu.Completed;
                CoreEndExecute();
                TryCallBack();
            }
        }

        internal void RegistCommandChanged(UnityAction<string, int, int> onActionObjStartExecute)
        {
            this.onActionObjStartExecute = onActionObjStartExecute;
        }

        public virtual void EndExecute()
        {
            if (statu != ExecuteStatu.Completed)
            {
                statu = ExecuteStatu.Completed;

                if(startHookCtrl.Statu != ExecuteStatu.Completed){
                    startHookCtrl.CoreEndExecute();
                }

                CoreEndExecute();

                if (completeHookCtrl.Statu != ExecuteStatu.Completed) {
                    completeHookCtrl.CoreEndExecute();
                }
            }
            else
            {
                Debug.Log("already completed" + StepName);
            }
        }
        public virtual void OnEndExecute()
        {
            if(log) Debug.Log("OnEndExecute", this);
            if (statu != ExecuteStatu.Completed)
            {
                Debug.Log(completeHookCtrl.Statu);
                if (completeHookCtrl.Statu == ExecuteStatu.Completed)
                {
                    statu = ExecuteStatu.Completed;
                    CoreEndExecute();
                    TryCallBack();
                }
                else if (completeHookCtrl.Statu == ExecuteStatu.UnStarted)
                {
                    completeHookCtrl.OnStartExecute(forceAuto);
                }
                else
                {
                    Debug.Log("wait:" + StepName);
                }
            }
            else
            {
                Debug.Log("already completed" + StepName);
            }
        }

        public void CoreEndExecute()
        {
            OnBeforeActionsPlayEnd();
            actionCtrl.SetContext(this);
            actionCtrl.OnEndExecute();
        }

        private void TryCallBack()
        {
            if (stepComplete != null)
                stepComplete.Invoke(StepName);
        }

        public virtual void UnDoExecute()
        {
            Debug.Log("UnDoExecute:"+this);
            statu = ExecuteStatu.UnStarted;

            if (completeHookCtrl.Statu != ExecuteStatu.UnStarted){
                completeHookCtrl.OnUnDoExecute();
            }

            OnBeforeActionsUnDo();
            actionCtrl.SetContext(this);
            actionCtrl.OnUnDoExecute();

            if(startHookCtrl.Statu != ExecuteStatu.UnStarted)
            {
                startHookCtrl.OnUnDoExecute();
            }

            if (enviromentCtrl != null){
                enviromentCtrl.OrignalState(environments);
            }
        }

        private void OnBeforeActionsStart()
        {
            if(enviromentCtrl != null){
                enviromentCtrl.StartState(environments);
            }

            if (commandBindingCtrl != null)
                commandBindingCtrl.OnBeforeActionsStart(this);
        }
        private void OnBeforeActionsUnDo()
        {
            if (commandBindingCtrl != null)
                commandBindingCtrl.OnBeforeActionsUnDo(this);
        }
        private void OnBeforeActionsPlayEnd()
        {
            if (commandBindingCtrl != null)
                commandBindingCtrl.OnBeforeActionsPlayEnd(this);

            if (enviromentCtrl != null){
                enviromentCtrl.CompleteState(environments);
            }
        }
    }
}

