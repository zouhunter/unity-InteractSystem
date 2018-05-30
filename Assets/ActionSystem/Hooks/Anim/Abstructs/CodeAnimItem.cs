using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace WorldActionSystem.Hooks
{
    public abstract class CodeAnimItem : AnimPlayer
    {
        [SerializeField]
        protected float time = 2f;
        protected Coroutine coroutine;
        [SerializeField]
        protected AnimationCurve animCurve;
    
        protected override void Start()
        {
            base.Start();
            InitState();
        }

        protected abstract void InitState();

        public override void StepActive()
        {
            base.StepActive();
            time = 1f / duration;
            coroutine = StartCoroutine(PlayAnim(onAutoPlayEnd));
        }

        public override void StepComplete()
        {
            base.StepComplete();

            StopAnim();
        }
        public override void StepUnDo()
        {
            base.StepUnDo();
            StopAnim();
        }

        protected virtual void StopAnim() {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
        protected float GetAnimValue(float value)
        {
            return animCurve.Evaluate(value);
        }
        protected abstract IEnumerator PlayAnim(UnityAction onComplete);
    }
}
