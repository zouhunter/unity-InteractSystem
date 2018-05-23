//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using UnityEngine.EventSystems;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using WorldActionSystem;
//namespace WorldActionSystem
//{
    
//    public abstract class ActionNode
//    {
//        [SerializeField,UnityEngine.Serialization.FormerlySerializedAs("m_name"),Attributes.DefultName]
//        protected string _name;
//        protected bool _completed;
//        public string Name
//        {
//            get
//            {
//                return _name;
//            }
//            set
//            {
//                _name = value;
//            }
//        }
//        public virtual bool Completed { get { return _completed; } protected set { _completed = value; } }
//        protected bool _started;
//        public virtual bool Started { get { return _started; } protected set { _started = value; } }
//        protected bool auto;
//        [SerializeField, Attributes.Range(0, 10)]
//        private int queueID;
//        public int QueueID
//        {
//            get
//            {
//                return queueID;
//            }
//        }
//        [SerializeField]
//        private bool _queueInAuto = true;
//        public bool QueueInAuto { get { return _queueInAuto; } }
//        [SerializeField,Attributes.DefultCameraAttribute()]
//        private string _cameraID;
//        public string CameraID { get { return _cameraID; } }
//        public Transform anglePos;
//        public UnityAction onEndExecute { get; set; }

//#if ActionSystem_G
//        [HideInInspector]
//#endif
//        public Toggle.ToggleEvent onStartExecute, onBeforeComplete;
//#if ActionSystem_G
//        [HideInInspector]
//#endif
//        public UnityEvent onUnDoExecute;
//        private ActionHook[] hooks;//外部结束钩子
//        public ActionHook[] Hooks { get { return hooks; } }
//        private HookCtroller hookCtrl;
//        protected AngleCtroller angleCtrl { get { return ActionSystem.Instence.angleCtrl; } }
//        private ActionGroup _system;
//        public ActionGroup system { get { return _system; } set { _system = value; } }
//        protected ElementController elementCtrl { get { return ElementController.Instence; } }
//        protected static List<ActionNode> startedList = new List<ActionNode>();
//        public abstract ControllerType CtrlType { get; }
//        public static bool log = false;
//        protected bool notice;

//        //protected virtual void Awake() { }

//        //protected virtual void Start()
//        //{
//        //    hooks = GetComponentsInChildren<ActionHook>(false);
//        //    if (hooks.Length > 0)
//        //    {
//        //        hookCtrl = new HookCtroller(this);
//        //    }
//        //    if (anglePos == null)
//        //    {
//        //        anglePos = transform;
//        //    }

//        //    WorpCameraID();
//        //    //gameObject.SetActive(startActive);
//        //}
//        protected virtual void OnDestroy() { }
//        protected virtual void Update()
//        {
//            if (Completed || !Started) return;

//            if (!Config.angleNotice/* || this is Actions.AnimObj*/) return;

//            if (notice)
//            {
//                angleCtrl.Notice(anglePos);
//            }
//            else
//            {
//                angleCtrl.UnNotice(anglePos);
//            }

//        }

//        public virtual void OnStartExecute(bool auto = false)
//        {
//            if (log) Debug.Log("OnStartExecute:" + this);
//            this.auto = auto;
//            if (!_started)
//            {
//                _started = true;
//                _completed = false;
//                notice = true;
//                //gameObject.SetActive(true);
//                startedList.Add(this);
//                OnStartExecuteInternal(auto);
//            }
//            else
//            {
//                Debug.LogError("already started");
//            }
//        }
//        public virtual void OnEndExecute(bool force)
//        {
//            notice = false;

//            if (force)
//            {
//                if (!Completed) CoreEndExecute(true);
//            }
//            else
//            {
//                if (hooks.Length > 0)
//                {
//                    if (hookCtrl.Complete)
//                    {
//                        if (!Completed) CoreEndExecute(false);
//                    }
//                    else if (!hookCtrl.Started)
//                    {
//                        hookCtrl.OnStartExecute(auto);
//                    }
//                    else
//                    {
//                        Debug.Log("wait:" + Name);
//                    }
//                }
//                else
//                {
//                    if (!Completed) CoreEndExecute(false);
//                }
//            }
//        }
//        protected virtual void OnBeforeEnd(bool force)
//        {
//            onBeforeComplete.Invoke(force);
//        }

//        private void CoreEndExecute(bool force)
//        {
//            angleCtrl.UnNotice(anglePos);

//            if (log) Debug.Log("OnEndExecute:" + this + ":" + force);

//            if (!_completed)
//            {
//                notice = false;
//                _started = true;
//                _completed = true;
//                startedList.Remove(this);
//                //gameObject.SetActive(endActive);

//                if (hooks.Length > 0)
//                {
//                    hookCtrl.OnEndExecute();
//                }

//                OnBeforeEnd(force);
//                if (log) Debug.Log("OnEndExecute:" + Name);

//                if (onEndExecute != null)
//                {
//                    onEndExecute.Invoke();
//                }
//            }
//            else
//            {
//                if (log) Debug.LogError("already completed");
//            }
//        }


//        public virtual void OnUnDoExecute()
//        {
//            angleCtrl.UnNotice(anglePos);

//            if (log) Debug.Log("OnUnDoExecute:" + this);

//            if (_started)
//            {
//                _started = false;
//                _completed = false;
//                notice = false;
//                OnUnDoExecuteInternal();
//                startedList.Remove(this);
//                //gameObject.SetActive(startActive);
//                if (hooks.Length > 0)
//                {
//                    hookCtrl.OnUnDoExecute();
//                }
//            }
//            else
//            {
//                Debug.LogError(this + "allready undo");
//            }

//        }

//        public int CompareTo(ActionNode other)
//        {
//            if (QueueID > other.QueueID)
//            {
//                return 1;
//            }
//            else if (QueueID < other.QueueID)
//            {
//                return -1;
//            }
//            else
//            {
//                return 0;
//            }
//        }

//        //private void WorpCameraID()
//        //{
//        //    if (string.IsNullOrEmpty(_cameraID))
//        //    {
//        //        var node = GetComponentInChildren<CameraNode>();
//        //        if (node != null)
//        //        {
//        //            _cameraID = node.name;
//        //        }
//        //    }
//        //}
//        private void OnStartExecuteInternal(bool auto)
//        {
//            this.onStartExecute.Invoke(auto);
//        }
//        private void OnUnDoExecuteInternal()
//        {
//            onUnDoExecute.Invoke();
//        }
//    }
//}