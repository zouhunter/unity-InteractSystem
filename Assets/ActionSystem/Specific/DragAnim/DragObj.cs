using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
#if !NoFunction
using DG.Tweening;
#endif
namespace WorldActionSystem
{
    public class DragObj : MonoBehaviour, IHightLightItem, IOutSideRegisterRender
    {
        [Range(1, 10)]
        public int animTime;
        //public bool startActive;
        public bool endActive;
        public bool Installed { get { return target != null; } }
        private DragPos target;
        public Renderer Render { get { return m_render; } }

        public UnityAction onInstallOkEvent { get; set; }
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
            startPos = transform.position;
            startRotation = transform.eulerAngles;
            if (m_render == null) m_render = GetComponentInChildren<Renderer>();
            //gameObject.SetActive(startActive);
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
        public void NormalInstall(DragPos target)
        {
#if !NoFunction

            if (!Installed)
            {
                //transform.rotation = target.transform.rotation;
                DoPath(target.transform.position, target.transform.eulerAngles, () =>
                {
                    OnInstallOK();
                });
                this.target = target;
            }
#endif
        }
        /// <summary>
        /// 定位安装
        /// </summary>
        /// <param name="target"></param>
        public void QuickInstall(DragPos target)
        {
            StopTween();
            if (!Installed)
            {
                transform.position = target.transform.position;
                transform.rotation = target.transform.rotation;
                OnInstallOK();
                this.target = target;
            }
        }

        public void NormalUnInstall()
        {
#if !NoFunction

            if (Installed)
            {
                //transform.rotation = startRotation;
                DoPath(startPos, startRotation, () =>
                {
                    OnUnInstallOK();
                });
                target = null;
            }
#endif
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
                target.Detach();
                target = null;
                OnUnInstallOK();
            }
#endif
        }

        public void OnPickUp()
        {
            //pickUpPos = transform.position;
            StopTween();
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
        private void OnInstallOK()
        {
            if (onInstallOkEvent != null)
                onInstallOkEvent();
        }
        private void OnUnInstallOK()
        {
            gameObject.SetActive(true);

            if (onUnInstallOkEvent != null)
                onUnInstallOkEvent();
        }
        public void RegisterRenderer(Renderer renderer)
        {
            if (renderer != null)
                m_render = renderer;
        }

        internal void TryHide()
        {
            gameObject.SetActive(endActive);
        }
    }

}