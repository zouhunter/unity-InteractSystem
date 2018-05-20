using System.Collections;

using UnityEngine;
using UnityEngine.Events;

namespace WorldActionSystem
{

    public class RotateAnim : CodeAnimItem
    {
        [SerializeField]
        protected Transform bodyTrans;
        [SerializeField]
        protected Transform center;

        [SerializeField]
        protected float angle;
        [SerializeField]
        protected AnimationCurve animCurve;

        protected Vector3 startCenterpostion;
        protected Vector3 startPosition;
        protected Quaternion startRotation;
        protected Vector3 axis;
        protected Vector3 targetPostion;
        protected Quaternion targetRotation;

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
            axis = center.transform.up;

            var fromDirection = bodyTrans.transform.position - center.position;
            targetPostion = bodyTrans.parent.InverseTransformPoint(Quaternion.AngleAxis(angle, axis) * fromDirection + bodyTrans.parent.TransformPoint(startCenterpostion));
            targetRotation = Quaternion.AngleAxis(angle, axis) * startRotation;
        }

        protected override IEnumerator PlayAnim(UnityAction onComplete)
        {
            float lastduration = 0;
            bodyTrans.localPosition = startPosition;
            bodyTrans.localRotation = startRotation;
            for (float i = 0; i < time; i += Time.deltaTime)
            {
                var duration = GetAnimValue(i / time);
                var currentAngle =( duration - lastduration) * angle;
                bodyTrans.RotateAround(bodyTrans.parent.TransformPoint(startCenterpostion), axis, currentAngle);
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
        }

        protected override void StopAnim()
        {
            base.StopAnim();
            bodyTrans.localPosition = startPosition;
            bodyTrans.localRotation = startRotation;
            bodyTrans.RotateAround(bodyTrans.parent.TransformPoint(startCenterpostion), axis, angle);
        }

        private float GetAnimValue(float value)
        {
            return animCurve.Evaluate(value);
        }

        private void OnDrawGizmos()
        {
            if (center != null && bodyTrans != null)
            {
                axis = center.transform.up;

                var fromDirection = bodyTrans.transform.position - center.position;
                var targetPosition = Quaternion.AngleAxis(angle, axis) * fromDirection + center.transform.position;
                Gizmos.DrawLine(center.position, targetPosition);
            }
        }
    }
}
