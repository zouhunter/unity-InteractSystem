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
    public class PickUpAbleElement : MonoBehaviour,IPickUpAbleItem,IInstallItem,IMatchItem, IHightLightItem, IOutSideRegisterRender
    {
        public int animTime { get { return Setting.installTime; } }
        public bool startActive = true;//如果是false，则到当前步骤时才会激活对象
        public bool endActive = true;//如果是false,则完成后将进行隐藏
        public bool Installed { get { return target != null; } }
        private GameObject target;
        public Renderer Render { get { return m_render; } }

        public UnityAction onInstallOkEvent;
        public UnityAction onUnInstallOkEvent;

        [SerializeField]
        private Renderer m_render;
#if !NoFunction
        private Vector3 startRotation;
        private Vector3 startPos;
        private Tweener move;
        private int smooth = 50;
#endif


#if !NoFunction
        void Start()
        {
            ActionSystem.Instance.CommandCtrl.RegistElement(this);
            if (m_render == null) m_render = gameObject.GetComponentInChildren<Renderer>();

            gameObject.layer = Setting.pickUpElementLayer;
            startPos = transform.position;
            startRotation = transform.eulerAngles;
            gameObject.SetActive(startActive);
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
#if !NoFunction
            DoPath(startPos, startRotation, () =>
            {
                if (onUnInstallOkEvent != null)
                    onUnInstallOkEvent();
            });
#endif
        }
        public void QuickMoveBack()
        {
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
        }

        /// <summary>
        /// 步骤激活（随机选中的一些installObj）
        /// </summary>
        public void StepActive()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 步骤结束（安装上之后整个步骤结束）
        /// </summary>
        public void StepComplete()
        {
            gameObject.SetActive(endActive);
        }
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void OnPickDown()
        {
#if !NoFunction
            move = transform.DOMove(startPos, animTime).SetAutoKill(true);
#endif
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
    }

}