using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem
{

    public class DragObj : ActionObj
    {
        [SerializeField, Header("target (child transform)")]
        private Transform targetHolder;
        private float autoDragTime { get { return Config.autoExecuteTime; } }
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Drag;
            }
        }
        private Coroutine waitCoroutine;
        public Vector3 targetPos { get; private set; }
        public Vector3 startPos { get; private set; }

        protected override void Start()
        {
            base.Start();
            InitPositions();
            gameObject.layer = Layers.dragPosLayer;
        }
        private void InitPositions()
        {
            startPos = transform.localPosition;
            targetPos = startPos + targetHolder.localPosition;
        }
        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            if (auto)
            {
                if (waitCoroutine == null)
                {
                    StartCoroutine(AutoDrag());
                }
            }
        }
        IEnumerator AutoDrag()
        {
            for (float i = 0; i < autoDragTime; i += Time.deltaTime)
            {
                transform.localPosition = Vector3.Lerp(startPos, targetPos, i / autoDragTime);
                yield return null;
            }
            OnEndExecute(false);
        }
        public override void OnEndExecute(bool force)
        {
            base.OnEndExecute(force);
            if (auto && waitCoroutine != null)
            {
                StopCoroutine(waitCoroutine);
                waitCoroutine = null;
            }
        }
        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            if (auto && waitCoroutine != null)
            {
                StopCoroutine(waitCoroutine);
                waitCoroutine = null;
            }
        }
    }

}