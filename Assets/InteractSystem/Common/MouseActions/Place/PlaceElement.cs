using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace InteractSystem.Actions
{
    /// <summary>
    /// 可操作对象具体行为实现
    /// </summary>
    public class PlaceElement : PickUpAbleItem, ISupportElement
    {

        private ActionGroup _system;
        public ActionGroup system { get { transform.SurchSystem(ref _system); return _system; } }
        protected ElementController elementCtrl { get { return ElementController.Instence; } }
        public int animTime { get { return Config.Instence.autoExecuteTime; } }
        //public bool startActive = true;//如果是false，则到当前步骤时才会激活对象
        public override bool OperateAble
        {
            get
            {
                return BindingObj == null;
            }
        }

        public GameObject ViewObj {
            get {
                if (m_viewObj == null)
                    return gameObject;
                return m_viewObj;
            }
        }

        public event UnityAction onInstallOkEvent;
        public event UnityAction onUnInstallOkEvent;

        [HideInInspector]
        public UnityEvent onStepActive, onStepComplete, onStepUnDo;
        [SerializeField,Attributes.DefultGameObject("显示对象")]
        private GameObject m_viewObj;
        protected Vector3 startRotation;
        protected Vector3 startPos;
        protected PathTweener move;
        protected int smooth = 50;
        protected bool actived;
        protected PlaceItem BindingObj
        {
            get
            {
                return targets.Count > 0 ? targets[0] as PlaceItem : null;
            }
        }

        protected bool hideOnInstall { get { return BindingObj ? BindingObj.hideOnInstall : false; } }//
        protected bool StraightMove { get { return BindingObj ? BindingObj.straightMove : false; } }
        protected bool IgnoreMiddle { get { return BindingObj ? BindingObj.ignoreMiddle : false; } }
        protected Transform Passby { get { return BindingObj ? BindingObj.passBy : null; } }
        protected bool tweening;
        protected UnityAction tweenCompleteAction;
        protected Vector3 lastPos;
        protected override void Awake()
        {
            base.Awake();
            move = new PathTweener(this);
        }
        protected override void Start()
        {
            base.Start();
            InitRender();
            startPos = transform.position;
            startRotation = transform.eulerAngles;
            gameObject.SetActive(startactive);
            elementCtrl.RegistElement(this);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            move.Kill();
            elementCtrl.RemoveElement(this);
        }
        
        protected virtual void InitRender()
        {
            if (m_viewObj == null)
                m_viewObj = gameObject;
        }

        protected virtual void CreatePosList(Vector3 end, Vector3 endRot, out List<Vector3> posList, out List<Vector3> rotList)
        {
            posList = new List<Vector3>();
            rotList = new List<Vector3>();
            Vector3 midPos = Vector3.zero;

            if (Passby != null)
            {
                midPos = Passby.position;
            }
            else
            {
                var player = FindObjectOfType<Camera>().transform;
                midPos = player.transform.position + player.transform.forward * Config.Instence.elementFoward;
            }

            var midRot = (endRot + transform.eulerAngles * 3) * 0.25f;
            if (StraightMove || IgnoreMiddle)
            {
                posList.Add(transform.position);
                rotList.Add(transform.eulerAngles);

                if (!IgnoreMiddle)
                {
                    posList.Add(midPos);
                    rotList.Add(midRot);
                }

                posList.Add(end);
                rotList.Add(endRot);
            }
            else
            {
                for (int i = 0; i < smooth; i++)
                {
                    float curr = (i + 0f) / (smooth - 1);
                    posList.Add(Bezier.CalculateBezierPoint(curr, transform.position, midPos, end));
                    rotList.Add(Bezier.CalculateBezierPoint(curr, transform.eulerAngles, midRot, endRot));
                }
            }

        }

        protected virtual void DoPath(Vector3 end, Vector3 endRot)
        {
            List<Vector3> poss;
            List<Vector3> rots;
            CreatePosList(end, endRot, out poss, out rots);
            tweening = true;
            move.DOPath(transform, poss.ToArray(), animTime, OnTweenComplete);
            move.OnWaypointChange((x) =>
            {
                transform.eulerAngles = rots[x];
            });
        }
        protected virtual void OnTweenComplete()
        {
            tweening = false;
            if (tweenCompleteAction != null)
            {
                tweenCompleteAction.Invoke();
            }
        }
        /// <summary>
        /// 动画安装
        /// </summary>
        /// <param name="target"></param>
        public virtual void NormalInstall(PlaceItem target, bool binding)
        {
            StopTween();
            if (OperateAble)
            {
                RecordPlayer(target);

                tweenCompleteAction = () =>
                {
                    OnInstallComplete();
                };

                DoPath(target.transform.position, target.transform.eulerAngles);

                if (!binding)
                {
                    UnBinding();
                }
            }
        }

        /// <summary>
        /// 定位安装
        /// </summary>
        /// <param name="target"></param>
        public virtual void QuickInstall(PlaceItem target, bool binding)
        {
            StopTween();
            if (OperateAble)
            {
                RecordPlayer(target);
                transform.position = target.transform.position;
                transform.rotation = target.transform.rotation;

                if (!binding)
                    UnBinding();

                OnInstallComplete();
            }
            else
            {
                Debug.LogError(this + "HaveBinding:" + BindingObj);
            }
        }
        /// <summary>
        /// 卸载
        /// </summary>
        public virtual void NormalUnInstall()
        {
            Debug.Log("NormalUnInstall");
            StopTween();
#if !NoFunction
            tweenCompleteAction = () =>
            {
                if (!OperateAble)
                {
                    UnBinding();
                }
                OnUnInstallComplete();
            };
            DoPath(startPos, startRotation);
#endif
        }

        /// <summary>
        /// 快速卸载
        /// </summary>
        public virtual void QuickUnInstall()
        {
            if(log) Debug.Log("QuickUnInstall:"+ gameObject,gameObject);

#if !NoFunction
            StopTween();
            transform.eulerAngles = startRotation;
            transform.position = startPos;
            UnBinding();

            if (!OperateAble)
            {
                UnBinding();
            }

            OnUnInstallComplete();
#endif
        }
        protected override void RegistPickupableEvents()
        {
            pickUpableFeature.RegistOnPickUp(OnPickUp);
            pickUpableFeature.RegistOnPickDown(OnPickDownOrStay);
            pickUpableFeature.RegistOnPickStay(OnPickDownOrStay);
            pickUpableFeature.RegistOnSetPosition((pos) => { transform.position = pos; });
            pickUpableFeature.RegistOnSetViweForward((forward) => { transform.forward = forward; });
        }
        /// <summary>
        /// 拿起事件
        /// </summary>
        protected void OnPickUp()
        {
            StopTween();
        }

        protected void OnPickDownOrStay()
        {
            StopTween();
        }

        /// <summary>
        /// 步骤激活（随机选中的一些installObj）
        /// </summary>
        protected override void OnSetActive(UnityEngine.Object target)
        {
            base.OnSetActive(target);
            actived = true;
            onStepActive.Invoke();
            gameObject.SetActive(true);
        }
        /// <summary>
        /// 步骤结束（安装上之后整个步骤结束）
        /// </summary>
        protected override void OnSetInActive(UnityEngine.Object target)
        {
            base.OnSetInActive(target);
            if (log){
                Debug.Log("StepComplete:" + Name, gameObject);
            }
            actived = false;
            onStepComplete.Invoke();
            if (tweening)
            {
                StopTween();
                OnTweenComplete();
            }
        }

        /// <summary>
        /// 步骤重置(没有用到的元素)
        /// </summary>
        public override void UnDoChanges(UnityEngine.Object target)
        {
            base.UnDoChanges(target);
            if (log)
                Debug.Log("StepUnDo:" + Name, gameObject);
            actived = false;
            onStepUnDo.Invoke();
            gameObject.SetActive(startactive);
        }


        protected virtual void StopTween()
        {
            move.Kill();
        }

        protected virtual void OnInstallComplete()
        {
            //if (hideOnInstall)
            //{
            //    gameObject.SetActive(false);
            //}
            if (onInstallOkEvent != null)
                onInstallOkEvent();
        }
        protected virtual void OnUnInstallComplete()
        {
            if (onUnInstallOkEvent != null)
                onUnInstallOkEvent();

            if (IsRuntimeCreated)
                Destroy(gameObject);
        }

        protected virtual PlaceItem UnBinding()
        {
            var old = BindingObj;
            RemovePlayer(old);
            return old;
        }

        public override void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }

}