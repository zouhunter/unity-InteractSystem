using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
using NodeGraph.DataModel;

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

        [SerializeField, Attributes.Range(0, 10)]
        private int queueID;
        public int QueueID
        {
            get
            {
                return queueID;
            }
        }
        [SerializeField]
        private bool _queueInAuto = true;
        public bool QueueInAuto { get { return _queueInAuto; } }
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
      
        private HookCtroller hookCtrl = new HookCtroller();
        private ActionGroup _system;
        protected static List<OperateNode> startedList = new List<OperateNode>();
        [SerializeField]
        private ActionHook[] hooks;//外部结束钩子
        [SerializeField]
        private ActionBinding[] binding;
        [SerializeField]
        public Enviroment[] environment;

        protected override void OnEnable()
        {
            base.OnEnable();
            statu = ExecuteStatu.UnStarted;
            InitHookCtrl();
        }
        private void InitHookCtrl()
        {
            hookCtrl.SetContext(this);
            hookCtrl.InitHooks(hooks);
            hookCtrl.onEndExecute += () =>
            {
                if (Statu != ExecuteStatu.Completed)
                {
                    OnEndExecute(false);
                }
            };
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
                startedList.Add(this);
                OnStartExecuteInternal(auto);
            }
            else
            {
                Debug.LogError("already started");
            }
        }
        public virtual void OnEndExecute(bool force)
        {
            OnBeforeEnd(force);

            if (force)
            {
                if (statu != ExecuteStatu.Completed)
                {
                    CoreEndExecute(true);
                }
            }
            else
            {
                if (hookCtrl.Statu == ExecuteStatu.Completed && statu != ExecuteStatu.Completed)
                {
                    CoreEndExecute(false);
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
        private void CoreEndExecute(bool force)
        {
            //angleCtrl.UnNotice(anglePos);
            if (log) Debug.Log("OnEndExecute:" + this + ":" + force);

            if (statu != ExecuteStatu.Completed)
            {
                statu = ExecuteStatu.Completed;
              
                if (hooks.Length > 0)
                {
                    hookCtrl.OnEndExecute();
                }

                if (onEndExecute != null)
                {
                    onEndExecute.Invoke();
                }
            }
            else
            {
                if (log) Debug.LogError("already completed");
            }
        }
        public virtual void OnUnDoExecute()
        {
            //angleCtrl.UnNotice(anglePos);

            if (log) Debug.Log("OnUnDoExecute:" + this);

            if (statu != ExecuteStatu.UnStarted)
            {
                statu = ExecuteStatu.UnStarted;
                OnUnDoExecuteInternal();
                //gameObject.SetActive(startActive);
                if (hooks.Length > 0)
                {
                    hookCtrl.OnUnDoExecute();
                }
            }
            else
            {
                Debug.LogError(this + "allready undo");
            }

        }

        public int CompareTo(OperateNode other)
        {
            if (QueueID > other.QueueID)
            {
                return 1;
            }
            else if (QueueID < other.QueueID)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }


        protected virtual void OnStartExecuteInternal(bool auto)
        {
        }
        protected virtual void OnBeforeEnd(bool force)
        {
            startedList.Remove(this);
        }
        protected virtual void OnUnDoExecuteInternal()
        {
            startedList.Remove(this);
        }
    }
}