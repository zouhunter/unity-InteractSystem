using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace InteractSystem.Hooks
{
    public class InstallAnim : CodeAnimItem
    {
        [SerializeField, Attributes.CustomField("本体")]
        protected Transform bodyTrans;
        [SerializeField, Attributes.CustomField("目标")]
        protected Transform targetTrans;

        [SerializeField, Attributes.CustomField("转速")]
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

        protected override void OnSetActive(UnityEngine.Object arg0)
        {
            base.OnSetActive(arg0);

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
            currentPos = startPosition;// bodyTrans.localPosition;
            currentRot = startRotation;// bodyTrans.localRotation;
            currentTargetPos = targetPosition - startPosition + currentPos;//bodyTrans.localPosition;
            currentTargetRot = targetRotation * Quaternion.Inverse(startRotation) * startRotation;//bodyTrans.localRotation;

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
