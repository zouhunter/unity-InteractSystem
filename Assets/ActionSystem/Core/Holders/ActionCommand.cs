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
        protected CommandBinding[] commandBindings;
        //环境对象
        [SerializeField]
        private Enviroment[] environments;
        [SerializeField]
        private ActionHook[] hooks;

        protected UnityAction<string, int, int> onActionObjStartExecute { get; set; }
        public string CameraID { get { return _cameraID; } }
        public string StepName { get { if (string.IsNullOrEmpty(_stepName)) _stepName = name; return _stepName; } }
        public ExecuteStatu Statu { get { return statu; } }
        private Events.OperateErrorAction userErr { get; set; }
        private UnityAction<string> stepComplete { get; set; }//步骤自动结束方法
        protected ActionCtroller ActionCtrl { get { return ActionCtroller.Instence; } }
        public ActionGroup Context { get; private set; }//上下文

        //执行状态
        protected ExecuteStatu statu = ExecuteStatu.UnStarted;
        //步骤控制器
        protected Structure.ActionStateMechine objectCtrl;
        //hook控制器
        protected HookCtroller hookCtrl = new HookCtroller();
        protected bool forceAuto;

        protected virtual void OnEnable()
        {
            statu = ExecuteStatu.UnStarted;
            objectCtrl = new Structure.ActionStateMechine(this);
            objectCtrl.onComplete = OnEndExecute;
            InitHookCtrl();
        }
        private void InitHookCtrl()
        {
            hookCtrl.SetContext(this);
            hookCtrl.InitHooks(hooks);
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
                ActionCtrl.SetContext(objectCtrl);
                ActionCtrl.OnStartExecute(forceAuto);
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
            Debug.Log("EndExecute", this);
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
            ActionCtrl.OnEndExecute();
        }

        private void TryCallBack()
        {
            if (stepComplete != null)
                stepComplete.Invoke(StepName);
        }

        public virtual void UnDoExecute()
        {
            statu = ExecuteStatu.UnStarted;

            if (hookCtrl.Statu != ExecuteStatu.Completed)
            {
                hookCtrl.OnUnDoExecute();
            }

            OnBeforeActionsUnDo();

            ActionCtrl.OnUnDoExecute();
        }
        private void OnBeforeActionsStart()
        {

        }
        private void OnBeforeActionsUnDo()
        {

        }
        private void OnBeforeActionsPlayEnd()
        {

        }
    }
}

