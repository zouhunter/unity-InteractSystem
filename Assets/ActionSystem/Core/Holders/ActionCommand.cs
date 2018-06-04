using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    [AddComponentMenu(MenuName.ActionCommand)]
    public class ActionCommand : NodeGraph.DataModel.NodeGraphObj
    {
        //步骤名
        [SerializeField, Attributes.DefultName]
        private string _stepName;
        //相机ID
        [SerializeField,Attributes.DefultCamera]
        private string _cameraID = CameraController.defultID;
        //功能绑定
        [SerializeField]
        protected Binding.CommandBinding[] commandBindings;
        //环境对象
        [SerializeField]
        private Enviroment.EnviromentItem[] environments;
        [SerializeField]
        private ActionHook[] hooks;

        protected UnityAction<string, int, int> onActionObjStartExecute { get; set; }
        public string CameraID { get { return _cameraID; } }
        public string StepName { get { if (string.IsNullOrEmpty(_stepName)) _stepName = name; return _stepName; } }
        public ExecuteStatu Statu { get { return statu; } }
        private Events.OperateErrorAction userErr { get; set; }
        private UnityAction<string> stepComplete { get; set; }//步骤自动结束方法
        protected ActionCtroller actionCtrl { get { return ActionCtroller.Instence; } }
        public ActionGroup Context { get; private set; }//上下文

        //执行状态
        protected ExecuteStatu statu = ExecuteStatu.UnStarted;
        //步骤控制器
        public Structure.ActionStateMechine objectCtrl { get; private set; }
        //hook控制器
        protected Hooks.HookCtroller hookCtrl;
        protected Binding.CommandBingCtrl commandBindingCtrl;
        protected Enviroment.EnviromentCtrl enviromentCtrl;
        protected bool forceAuto;

        protected virtual void OnEnable()
        {
            statu = ExecuteStatu.UnStarted;
            InitOperateCtrl();
            InitHookCtrl();
            InitBindgCtrl();
            InitEnviromentCtrl();
        }

        private void InitEnviromentCtrl()
        {
            if(environments != null && environments.Length > 0)
            {
                enviromentCtrl = new Enviroment.EnviromentCtrl(environments);
            }
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
            hookCtrl = new WorldActionSystem.Hooks.HookCtroller(hooks);
            hookCtrl.onEndExecute += OnHookComplete;
        }

        public void SetContext(ActionGroup group)
        {
            this.Context = group;
            //重置当前command的信息
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
                OnBeforeActionsStart();
                actionCtrl.SetContext(this);
                actionCtrl.OnStartExecute(forceAuto);
                return true;
            }
            else
            {
                Debug.Log("already started" + name);
                return false;
            }
        }

        private void OnHookComplete()
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

                CoreEndExecute();

                if (hookCtrl.Statu != ExecuteStatu.Completed) {
                    hookCtrl.OnEndExecute();
                }
            }
            else
            {
                Debug.Log("already completed" + StepName);
            }
        }
        public virtual void OnEndExecute()
        {
            Debug.Log("OnEndExecute", this);
            if (statu != ExecuteStatu.Completed)
            {
                if (hookCtrl.Statu == ExecuteStatu.Completed)
                {
                    statu = ExecuteStatu.Completed;
                    CoreEndExecute();
                }
                else if (hookCtrl.Statu == ExecuteStatu.UnStarted)
                {
                    hookCtrl.OnStartExecute(forceAuto);
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

            if (hookCtrl.Statu != ExecuteStatu.UnStarted){
                hookCtrl.OnUnDoExecute();
            }

            OnBeforeActionsUnDo();
            actionCtrl.OnUnDoExecute();
            if (enviromentCtrl != null){
                enviromentCtrl.OrignalState();
            }
        }
        private void OnBeforeActionsStart()
        {
            if(enviromentCtrl != null){
                enviromentCtrl.StartState();
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
                enviromentCtrl.CompleteState();
            }
        }
    }
}

