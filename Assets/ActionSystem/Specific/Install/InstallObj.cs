using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace WorldActionSystem
{
    /// <summary>
    /// 可操作对象具体行为实现
    /// </summary>
    public class InstallObj : MonoBehaviour, IHightLightItem, IOutSideRegisterRender
    {
        [Range(1, 10)]
        public int animTime;
        public MidlePosRot midPosRot;
        public bool Installed { get { return target != null; } }
        public Renderer Render { get { return m_render; } }

        private InstallPos target;
        private Vector3 startPos;
        private Vector3 startRotation;
        public UnityAction onInstallOkEvent;
        public UnityAction onUnInstallOkEvent;
        [SerializeField]
        private Renderer m_render;
        private Tweener move;

        void Start()
        {
            startPos = transform.position;
            startRotation = transform.eulerAngles;
            if (m_render == null) m_render = GetComponentInChildren<Renderer>();
        }

        private void DoPath(Vector3 end, Vector3 endRot, TweenCallback onComplete)
        {
            List<Vector3> path = new List<Vector3>();
            path.Add(transform.position);
            if(midPosRot.pos != null && midPosRot.pos.Count > 0) path.AddRange(midPosRot.pos);
            path.Add(end);

            move = transform.DOPath(path.ToArray(), animTime).OnComplete(onComplete).SetAutoKill(true);
            if (midPosRot.rot.Count == 0)
            {
                transform.eulerAngles = endRot;
            }
            else
            {
                move.OnWaypointChange((x) =>
                {
                    MidlePosRot.Rot rot = midPosRot.rot.Find(i => i.id == x);
                    if (rot != null)
                    {
                        transform.eulerAngles = rot.rot;
                    }
                });
            }
        }

        /// <summary>
        /// 动画安装
        /// </summary>
        /// <param name="target"></param>
        public void NormalInstall(InstallPos target)
        {
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
        }
        /// <summary>
        /// 定位安装
        /// </summary>
        /// <param name="target"></param>
        public void QuickInstall(InstallPos target)
        {
            StopTween();
            if (!Installed)
            {
                transform.position = target.transform.position;
                transform.rotation = target.transform.rotation;
                this.target = target;
            }
        }

        public void NormalUnInstall()
        {
            if (Installed)
            {
                //transform.rotation = startRotation;
                DoPath(startPos, startRotation, () =>
                {
                    if (onUnInstallOkEvent != null)
                        onUnInstallOkEvent();
                });
                target = null;
            }
        }
        public void QuickUnInstall()
        {
            StopTween();
            if (Installed)
            {
                move.Pause();
                transform.eulerAngles = startRotation;
                transform.position = startPos;
                target.Detach();
                target = null;
            }
        }

        public void OnPickUp()
        {
            //pickUpPos = transform.position;
        }

        public void OnPickDown()
        {
            transform.DOMove(startPos, animTime).SetAutoKill(true);
        }

        private void StopTween()
        {
            move.Rewind();
            move.Pause();
            onUnInstallOkEvent = null;
            onInstallOkEvent = null;
        }

        public void RegisterRenderer(Renderer renderer)
        {
            if(renderer != null)
            m_render = renderer;
        }
    }

}