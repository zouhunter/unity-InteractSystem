using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace InteractSystem.Hooks
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

        protected Vector3 currentTargetPos;
        protected Quaternion currentTargetRot;
        protected Vector3 currentPos;
        protected Quaternion currentRot;

        protected override void InitState()
        {
            startPosition = bodyTrans.localPosition;
            startRotation = bodyTrans.localRotation;

            targetPosition = bodyTrans.transform.parent.InverseTransformPoint(targetTrans.transform.position);
            targetRotation = Quaternion.Euler(bodyTrans.transform.parent.InverseTransformVector(targetTrans.transform.eulerAngles));
        }

        public override void SetInActive(UnityEngine.Object target)
        {
            base.SetInActive(target);

            if (reverse)
            {
                bodyTrans.localPosition = currentPos;
                bodyTrans.localRotation = currentRot;
            }
            else
            {
                bodyTrans.localPosition = currentTargetPos;
                bodyTrans.localRotation = currentTargetRot;
            }
        }

        public override void UnDoChanges(UnityEngine.Object target)
        {
            base.UnDoChanges(target);

            bodyTrans.localPosition = startPosition;
            bodyTrans.localRotation = startRotation;
        }

        protected override IEnumerator PlayAnim(UnityAction onComplete)
        {
            currentPos = bodyTrans.localPosition;
            currentRot = bodyTrans.localRotation;
            currentTargetPos = targetPosition - startPosition + bodyTrans.localPosition;
            currentTargetRot = targetRotation * Quaternion.Inverse(startRotation) * bodyTrans.localRotation;

            var startPos = reverse ? currentTargetPos : currentPos;
            var targetPos = reverse ? currentPos : currentTargetPos;
            var targetRot = reverse ? currentRot : currentTargetRot;

            var dir = reverse ? startPosition - targetPosition : targetPosition - startPosition;
            var rot = Quaternion.AngleAxis(rotateSpeed, dir);
            for (float i = 0; i < time; i += Time.deltaTime)
            {
                bodyTrans.localPosition = Vector3.Lerp(startPos, targetPos, GetAnimValue(i / time));
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
