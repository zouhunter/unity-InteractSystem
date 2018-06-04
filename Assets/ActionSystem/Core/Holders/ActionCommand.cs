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

        protected virtual void OnEnable()
        {
            statu = ExecuteStatu.UnStarted;
            objectCtrl = new Structure.ActionStateMechine(this);
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

        /// <summary>
        /// 操作过程自动结束
        /// </summary>
        internal bool Complete()
        {
            if (statu != ExecuteStatu.Completed)
            {
                statu = ExecuteStatu.Completed;
                OnEndExecute();
                if (stepComplete != null)
                    stepComplete.Invoke(StepName);
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

        internal void RegistCommandChanged(UnityAction<string, int, int> onActionObjStartExecute)
        {
            this.onActionObjStartExecute = onActionObjStartExecute;
        }

        /// <summary>
        /// 强制结束
        /// </summary>
        public virtual bool EndExecute()
        {
            //Debug.Log("EndExecute", gameObject);

            if (statu != ExecuteStatu.Completed)
            {
                statu = ExecuteStatu.Completed;
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
            OnBeforeActionsPlayEnd();
            ActionCtrl.OnEndExecute();
        }

        public virtual void UnDoExecute()
        {
            statu = ExecuteStatu.UnStarted;
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

