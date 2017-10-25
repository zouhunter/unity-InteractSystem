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
    public class PickUpAbleElement : MonoBehaviour, IPickUpAbleItem,IOutSideRegisterRender
    {
        public int animTime { get { return Setting.installTime; } }
        public bool startActive = true;//如果是false，则到当前步骤时才会激活对象
        public bool endActive = true;//如果是false,则完成后将进行隐藏
        public bool Installed { get { return target != null; } }
        private GameObject target;
        public Renderer Render { get { return m_render; } }

        public UnityAction onInstallOkEvent;
        public UnityAction onUnInstallOkEvent;

        public UnityEvent onPickUp;
        public UnityEvent OnLayDown;

        public UnityEvent onStepActive;
        public UnityEvent onStepComplete;
        public UnityEvent onStepUnDo;

        [SerializeField]
        private Renderer m_render;
        [SerializeField]
        protected Color highLightColor = Color.green;
#if !NoFunction
        private Vector3 startRotation;
        private Vector3 startPos;
        private Tweener move;
        private int smooth = 50;
#endif
        private IHighLightItems highLighter;
        private bool actived;
#if !NoFunction
        void Start()
        {
            ElementController.RegistElement(this);
            InitRender();
            gameObject.layer = Setting.pickUpElementLayer;
            startPos = transform.position;
            startRotation = transform.eulerAngles;
            gameObject.SetActive(startActive);
        }
        private void InitRender()
        {
            if (m_render == null) m_render = gameObject.GetComponentInChildren<Renderer>();
            highLighter = new ShaderHighLight();
        }
        private void CreatePosList(Vector3 end, Vector3 endRot, out List<Vector3> posList, out List<Vector3> rotList)
        {
            posList = new List<Vector3>();
            rotList = new List<Vector3>();
            var player = FindObjectOfType<Camera>().transform;
            var midPos = player.transform.position + player.transform.forward * 4f;
            var midRot = (endRot + transform.eulerAngles * 3) * 0.25f;
            for (int i = 0; i < smooth; i++)
            {
                float curr = (i + 0f) / (smooth - 1);
                posList.Add(Bezier.CalculateBezierPoint(curr, transform.position, midPos, end));
                rotList.Add(Bezier.CalculateBezierPoint(curr, transform.eulerAngles, midRot, endRot));
            }
        }

        private void DoPath(Vector3 end, Vector3 endRot, TweenCallback onComplete)
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
        public void NormalInstall(GameObject target)
        {
            StopTween();
#if !NoFunction
            if (!Installed)
            {
                //transform.rotation = target.transform.rotation;
                DoPath(target.transform.position, target.transform.eulerAngles, () =>
                {
                    if (onInstallOkEvent != null)
                        onInstallOkEvent();
                });
                this.target = target;
            }
#endif
        }
        public void NormalMoveTo(GameObject target)
        {
            StopTween();
#if !NoFunction
            DoPath(target.transform.position, target.transform.eulerAngles, () =>
            {
                if (onInstallOkEvent != null)
                    onInstallOkEvent();
            });
#endif
        }
        public void QuickMoveTo(GameObject target)
        {
            StopTween();

            transform.position = target.transform.position;
            transform.rotation = target.transform.rotation;

            if (onInstallOkEvent != null)
                onInstallOkEvent();
        }

        /// <summary>
        /// 定位安装
        /// </summary>
        /// <param name="target"></param>
        public void QuickInstall(GameObject target)
        {
            StopTween();
            if (!Installed)
            {
                transform.position = target.transform.position;
                transform.rotation = target.transform.rotation;
                if (onInstallOkEvent != null)
                    onInstallOkEvent();
                this.target = target;
            }
        }

        public void NormalUnInstall()
        {
            StopTween();
#if !NoFunction
            if (Installed)
            {
                DoPath(startPos, startRotation, () =>
                {
                    if (onUnInstallOkEvent != null)
                        onUnInstallOkEvent();
                });
                target = null;
            }
#endif
        }
        public void NormalMoveBack()
        {
            StopTween();
#if !NoFunction
            if (OnLayDown != null) OnLayDown.Invoke();

            DoPath(startPos, startRotation, () =>
            {
                if (onUnInstallOkEvent != null)
                    onUnInstallOkEvent();
            });
#endif
        }
        public void QuickMoveBack()
        {
            StopTween();

            if (OnLayDown != null) OnLayDown.Invoke();

            transform.eulerAngles = startRotation;
            transform.position = startPos;

            if (onUnInstallOkEvent != null)
                onUnInstallOkEvent();
        }

        public void QuickUnInstall()
        {
#if !NoFunction
            StopTween();
            if (Installed)
            {
                move.Pause();
                transform.eulerAngles = startRotation;
                transform.position = startPos;
                target = null;
                if (onUnInstallOkEvent != null)
                    onUnInstallOkEvent();
            }
#endif
        }

        public void OnPickUp()
        {
            StopTween();

            if (onPickUp != null)
                onPickUp.Invoke();
        }

        /// <summary>
        /// 步骤激活（随机选中的一些installObj）
        /// </summary>
        public void StepActive()
        {
            actived = true;
            gameObject.SetActive(true);
            onStepActive.Invoke();
        }
        /// <summary>
        /// 步骤结束（安装上之后整个步骤结束）
        /// </summary>
        public void StepComplete()
        {
            actived = false;
            gameObject.SetActive(endActive);
            onStepComplete.Invoke();
        }
        /// <summary>
        /// 步骤重置(没有用到的元素)
        /// </summary>
        public void StepUnDo()
        {
            actived = false;
            gameObject.SetActive(startActive);
            onStepUnDo.Invoke();
        }

        private void Update()
        {
            if (!Setting.highLightNotice) return;
            if(actived)
            {
                highLighter.HighLightTarget(m_render, highLightColor);
            }
            else
            {
                highLighter.UnHighLightTarget(m_render);
            }
        }

        private void StopTween()
        {
#if !NoFunction
            move.Pause();
            move.Kill(true);
#endif
        }

        public void RegisterRenderer(Renderer renderer)
        {
            if (renderer != null)
                m_render = renderer;
        }
        void OnDestroy()
        {
            StopTween();
        }
    }

}