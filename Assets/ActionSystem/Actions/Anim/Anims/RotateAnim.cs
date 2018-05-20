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
        }

        protected override IEnumerator PlayAnim(UnityAction onComplete)
        {
            for (float i = 0; i < time; i += Time.deltaTime)
            {
                var duration = GetAnimValue(i / time);
                var currentAngle = duration * angle;
                SetTargetPostionAndRotation(currentAngle);
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
            SetTargetPostionAndRotation(0);
        }

        protected override void StopAnim()
        {
            base.StopAnim();
            SetTargetPostionAndRotation(angle);
        }

        private void SetTargetPostionAndRotation(float angle)
        {
            var fromDirection = startPosition - startCenterpostion;
            bodyTrans.localPosition = Quaternion.AngleAxis(angle, axis) * fromDirection + startCenterpostion;
            bodyTrans.localRotation = Quaternion.AngleAxis(angle, axis) * startRotation;
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
                var centerLocalPos = bodyTrans.parent.InverseTransformPoint(center.transform.position);
                var fromDirection = bodyTrans.transform.localPosition - centerLocalPos;
                var targetPosition = Quaternion.AngleAxis(angle, axis) * fromDirection + centerLocalPos;
                Gizmos.DrawLine(center.position, targetPosition);
            }
        }
    }
}
