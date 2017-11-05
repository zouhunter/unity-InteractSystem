using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
#if !NoFunction
using DG.Tweening;
#endif
namespace WorldActionSystem
{

    /// <summary>
    /// 可操作对象具体行为实现
    /// </summary>
    public class PickUpAbleElement : MonoBehaviour, IPickUpAbleItem, IPlaceItem, IRuntimeActive
    {
        public string _name;
        public int animTime { get { return Setting.autoExecuteTime; } }
        public bool startActive = true;//如果是false，则到当前步骤时才会激活对象
        public bool HaveBinding { get { return target != null; } }
        public virtual string Name { get { return _name; } }

        public bool Started { get { return actived; } }

        public Renderer Render { get { return m_render; } }
        public event UnityAction onInstallOkEvent;
        public event UnityAction onUnInstallOkEvent;

        public UnityEvent onPickUp;
        public UnityEvent onLayDown;

        public UnityEvent onStepActive;
        public UnityEvent onStepComplete;
        public UnityEvent onStepUnDo;

        [SerializeField]
        private Renderer m_render;
        [SerializeField]
        protected Color highLightColor = Color.green;
#if !NoFunction
        protected Vector3 startRotation;
        protected Vector3 startPos;
        protected Tweener move;
        protected int smooth = 50;
#endif
        protected IHighLightItems highLighter;
        protected bool actived;

        protected PlaceObj target;
        public PlaceObj BindingObj { get { return target; } }
        protected bool hideOnInstall { get { return target? target.hideOnInstall:false; } }//
        protected bool StraightMove { get { return target ? target.straightMove:false ; } }
        protected bool IgnoreMiddle { get { return target ? target.ignoreMiddle : false; } }
        protected Transform Passby { get { return target?target.passBy:null; } }

#if !NoFunction
        protected virtual void Start()
        {
            if (string.IsNullOrEmpty(_name)) _name = name;
            ElementController.RegistElement(this);
            InitRender();
            gameObject.layer = Setting.pickUpElementLayer;
            startPos = transform.position;
            startRotation = transform.eulerAngles;
            gameObject.SetActive(startActive);
        }

        protected virtual void InitRender()
        {
            if (m_render == null) m_render = gameObject.GetComponentInChildren<Renderer>();
            highLighter = new ShaderHighLight();
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
                midPos = player.transform.position + player.transform.forward * Setting.elementFoward;
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

        protected virtual void DoPath(Vector3 end, Vector3 endRot, TweenCallback onComplete)
        {
            List<Vector3> poss;
            List<Vector3> rots;
            CreatePosList(end, endRot, out poss, out rots);
            move = transform.DOPath(poss.ToArray(), animTime).OnComplete(onComplete).SetAutoKill(true);
            move.OnWaypointChange((x) =>
            {
                transform.eulerAngles = rots[x];
            });
        }
#endif
        /// <summary>
        /// 动画安装
        /// </summary>
        /// <param name="target"></param>
        public virtual void NormalInstall(PlaceObj target, bool complete = true,bool binding = true)
        {
            StopTween();
#if !NoFunction
            if (!HaveBinding)
            {
                Binding(target);
                DoPath(target.transform.position, target.transform.eulerAngles, () =>
                {
                    OnInstallComplete(complete);
                });
                if(!binding)
                {
                    UnBinding();
                }
            }
#endif
        }

        /// <summary>
        /// 定位安装
        /// </summary>
        /// <param name="target"></param>
        public virtual void QuickInstall(PlaceObj target, bool complete = true,bool binding = true)
        {
            StopTween();
            if (!HaveBinding)
            {
                Binding(target);
                transform.position = target.transform.position;
                transform.rotation = target.transform.rotation;

                if (!binding)
                    UnBinding();

                OnInstallComplete(complete);
            }
            else
            {
                Debug.LogError(this +"HaveBinding:" + BindingObj);
            }
        }

        /// <summary>
        /// 卸载
        /// </summary>
        public virtual void NormalUnInstall()
        {
            StopTween();
#if !NoFunction

            DoPath(startPos, startRotation, () =>
            {
                OnUnInstallComplete();

                if (HaveBinding)
                {
                    UnBinding();
                }
            });
#endif
        }

        /// <summary>
        /// 快速卸载
        /// </summary>
        public virtual void QuickUnInstall()
        {
#if !NoFunction
            StopTween();
            move.Pause();
            transform.eulerAngles = startRotation;
            transform.position = startPos;
            target = null;

            OnUnInstallComplete();

            if (HaveBinding)
            {
                UnBinding();
            }
#endif
        }

        /// <summary>
        /// 拿起事件
        /// </summary>
        public virtual void OnPickUp()
        {
            StopTween();

            if (onPickUp != null)
            {
                onPickUp.Invoke();
            }
        }


        public virtual void OnPickDown()
        {
            StopTween();
            if(onLayDown != null)
            {
                onLayDown.Invoke();
            }
            NormalUnInstall();
        }

        /// <summary>
        /// 步骤激活（随机选中的一些installObj）
        /// </summary>
        public virtual void StepActive()
        {
            actived = true;
            onStepActive.Invoke();
            gameObject.SetActive(true);
        }
        /// <summary>
        /// 步骤结束（安装上之后整个步骤结束）
        /// </summary>
        public virtual void StepComplete()
        {
            actived = false;
            onStepComplete.Invoke();
        }

        /// <summary>
        /// 步骤重置(没有用到的元素)
        /// </summary>
        public virtual void StepUnDo()
        {
            actived = false;
            onStepUnDo.Invoke();
            gameObject.SetActive(startActive);
        }

        protected virtual void Update()
        {
            if (!Setting.highLightNotice) return;
            if (m_render == null) return;
            if (actived)
            {
                highLighter.HighLightTarget(m_render, highLightColor);
            }
            else
            {
                highLighter.UnHighLightTarget(m_render);
            }
        }

        protected virtual void StopTween()
        {
#if !NoFunction
            move.Pause();
            move.Kill(true);
#endif
        }

        protected virtual void OnInstallComplete(bool complete)
        {
            if(hideOnInstall){
                gameObject.SetActive(false);
            }

            if (onInstallOkEvent != null)
                onInstallOkEvent();

            if (complete) StepComplete();
        }
        protected virtual void OnUnInstallComplete()
        {
            if (onUnInstallOkEvent != null)
                onUnInstallOkEvent();
        }
        protected virtual void Binding(PlaceObj target)
        {
            this.target = target;
        }
        protected virtual PlaceObj UnBinding()
        {
            var old = target;
            target = null;
            return old;
        }
       
        protected virtual void OnDestroy()
        {
            StopTween();
        }
    }

}