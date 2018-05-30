using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace WorldActionSystem.Hooks
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
            startPosition = bodyTrans.localPosition;
            startRotation = bodyTrans.localRotation;

            targetPosition = bodyTrans.transform.parent.InverseTransformPoint(targetTrans.transform.position);
            targetRotation = Quaternion.Euler(bodyTrans.transform.parent.InverseTransformVector(targetTrans.transform.eulerAngles));
        }

        public override void StepComplete()
        {
            base.StepComplete();

            if (reverse)
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
            var startPos = reverse ? targetPosition : startPosition;
            var targetPos = reverse ? startPosition : targetPosition;
            var targetRot = reverse ? startRotation : targetRotation;

            var dir = reverse ? startPosition - targetPosition : targetPosition - startPosition;
            var rot = Quaternion.AngleAxis(rotateSpeed, dir);
            for (float i = 0; i < time; i += Time.deltaTime)
            {
                bodyTrans.localPosition = Vector3.Lerp(startPos, targetPos,GetAnimValue( i / time));
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
            if (bodyTrans)
                bodyTrans.gameObject.SetActive(visible);
        }
    }
}
