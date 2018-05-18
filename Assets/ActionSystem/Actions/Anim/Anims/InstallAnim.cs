﻿using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace WorldActionSystem
{
    public class InstallAnim : CodeAnimItem
    {
        [SerializeField]
        protected Transform bodyTrans;
        [SerializeField]
        protected Transform targetTrans;
     
        [SerializeField, Range(-10, 10)]
        protected float rotateSpeed;

        protected Vector3 startPosition;
        protected Quaternion startRotation;
        protected Vector3 targetPosition;
        protected Quaternion targetRotation;

        protected override void InitState()
        {
            if (bodyTrans == null) bodyTrans = transform;
            if (targetTrans == null) targetTrans = transform;

            startPosition = bodyTrans.localPosition;
            startRotation = bodyTrans.localRotation;

            targetPosition = targetTrans.transform.localPosition;
            targetRotation = targetTrans.transform.localRotation;
        }

        public override void StepComplete()
        {
            base.StepComplete();

            if (from)
            {
                bodyTrans.localPosition = startPosition;
                bodyTrans.localRotation = startRotation;
            }
            else
            {
                bodyTrans.localPosition = targetPosition;
                bodyTrans.localRotation = targetRotation;
            }
        }

        public override void StepUnDo()
        {
            base.StepUnDo();

            bodyTrans.localPosition = startPosition;
            bodyTrans.localRotation = startRotation;
        }

        protected override IEnumerator PlayAnim(UnityAction onComplete)
        {
            var startPos = from ? targetPosition : startPosition;
            var targetPos = from ? startPosition : targetPosition;
            var targetRot = from ? startRotation : targetRotation;

            var dir = from ? startPosition - targetPosition : targetPosition - startPosition;
            var rot = Quaternion.AngleAxis(rotateSpeed, dir);
            for (float i = 0; i < time; i += Time.deltaTime)
            {
                bodyTrans.localPosition = Vector3.Lerp(startPos, targetPos, i / time);
                bodyTrans.localRotation = rot * bodyTrans.localRotation;
                yield return null;
            }

            bodyTrans.localRotation = targetRot;

            if (onComplete != null)
            {
                onComplete.Invoke();
                onComplete = null;
            }
        }

        public override void SetVisible(bool visible)
        {
            if(bodyTrans)
                bodyTrans.gameObject.SetActive(visible);
        }
    }
}
