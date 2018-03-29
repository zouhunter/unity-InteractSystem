using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace WorldActionSystem
{
    public class InstallAnim : AnimPlayer
    {
        [SerializeField]
        protected Transform bodyTrans;
        [SerializeField]
        protected Transform targetTrans;
        [SerializeField, Range(-10, 10)]
        protected float rotateSpeed;
        [SerializeField]
        protected float time = 2f;

        protected Vector3 startPosition;
        protected Quaternion startRotation;
        protected Coroutine coroutine;
        protected Vector3 targetPosition;
        protected Quaternion targetRotation;
        protected override void Awake()
        {
            base.Awake();
            startPosition = bodyTrans.localPosition;

            if (targetTrans != null)
            {
                targetPosition = targetTrans.transform.localPosition;
                targetRotation = targetTrans.transform.localRotation;
            }
        }

        public override void StepActive()
        {
            time = 1f / duration;
            bodyTrans.localPosition = startPosition;
            coroutine = StartCoroutine(MoveAnim(onAutoPlayEnd));
        }

        public override void StepComplete()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
            bodyTrans.localPosition = targetPosition;
            bodyTrans.localRotation = targetRotation;
        }

        public override void StepUnDo()
        {
            Debug.Log("UnDoPlay");
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
            bodyTrans.localPosition = startPosition;
            bodyTrans.localRotation = startRotation;
        }

        public override void SetPosition(Vector3 pos)
        {
            transform.position = pos;
        }

        protected IEnumerator MoveAnim(UnityAction onComplete)
        {
            var startPos = bodyTrans.localPosition;
            var startRot = bodyTrans.localRotation;
            var dir = targetPosition - startPos;
            var rot = Quaternion.AngleAxis(rotateSpeed, dir);
            for (float i = 0; i < time; i += Time.deltaTime)
            {
                bodyTrans.localPosition = Vector3.Lerp(startPos, targetPosition, i / time);
                bodyTrans.localRotation = rot * bodyTrans.localRotation;
                yield return null;
            }
          
            bodyTrans.localPosition = targetPosition;
            bodyTrans.localRotation = targetRotation;

            if (onComplete != null)
            {
                onComplete.Invoke();
                onComplete = null;
            }
        }


    }
}
