using System.Collections;

using UnityEngine;
using UnityEngine.Events;

namespace InteractSystem.Hooks
{
    public class RotateAnim : CodeAnimItem
    {
        [SerializeField]
        protected Transform bodyTrans;
        [SerializeField]
        protected Transform center;

        [SerializeField]
        protected float angle;
        protected Vector3 startCenterpostion;
        protected Vector3 startPosition;
        protected Quaternion startRotation;
        public override void SetVisible(bool visible)
        {
            bodyTrans.gameObject.SetActive(visible);
        }

        protected override void InitState()
        {
            if (bodyTrans == null)
                bodyTrans = transform;
            if (center == null)
                center = transform;

            startPosition = bodyTrans.localPosition;
            startRotation = bodyTrans.localRotation;
            startCenterpostion = bodyTrans.parent.InverseTransformPoint(center.transform.position);
        }

        protected override IEnumerator PlayAnim(UnityAction onComplete)
        {
            float lastduration = 0;
            bodyTrans.localPosition = startPosition;
            bodyTrans.localRotation = startRotation;
            var axis = center.transform.up;

            if (reverse)
            {
                bodyTrans.RotateAround(bodyTrans.parent.TransformPoint(startCenterpostion), axis, angle);
            }

            for (float i = 0; i < time; i += Time.deltaTime)
            {
                var duration = GetAnimValue(i / time);
                var currentAngle = (duration - lastduration) * angle;
                bodyTrans.RotateAround(bodyTrans.parent.TransformPoint(startCenterpostion), reverse ? -axis : axis, currentAngle);
                lastduration = duration;
                yield return null;
            }

            if (onComplete != null)
            {
                onComplete.Invoke();
                onComplete = null;
            }
        }

        public override void StepUnDo()
        {
            base.StepUnDo();

            bodyTrans.localPosition = startPosition;
            bodyTrans.localRotation = startRotation;

            if (reverse)
            {
                var axis = center.transform.up;
                bodyTrans.RotateAround(bodyTrans.parent.TransformPoint(startCenterpostion), axis, angle);
            }
        }

        protected override void StopAnim()
        {
            base.StopAnim();
            bodyTrans.localPosition = startPosition;
            bodyTrans.localRotation = startRotation;
            if (!reverse){
                var axis = center.transform.up;// bodyTrans.parent.TransformPoint(startPosition) - bodyTrans.parent.TransformPoint(startCenterpostion);
                bodyTrans.RotateAround(bodyTrans.parent.TransformPoint(startCenterpostion), axis, angle);
            }
        }

        private void OnDrawGizmos()
        {
            if (center != null && bodyTrans != null)
            {
                var axis = center.transform.up;

                var fromDirection = bodyTrans.transform.position - center.position;
                var targetPosition = Quaternion.AngleAxis(angle, axis) * fromDirection + center.transform.position;
                Gizmos.DrawLine(center.position, targetPosition);
            }
        }
    }
}
