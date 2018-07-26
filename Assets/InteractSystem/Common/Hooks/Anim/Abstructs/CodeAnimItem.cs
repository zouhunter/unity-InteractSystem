using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using InteractSystem.Graph;

namespace InteractSystem.Hooks
{
    public abstract class CodeAnimItem : AnimPlayer
    {
        [SerializeField, Attributes.CustomField("默认时间")]
        protected float time = 2f;
        protected Coroutine coroutine;
        [SerializeField, Attributes.CustomField("动画曲线")]
        protected AnimationCurve animCurve;
    
        protected override void Start()
        {
            base.Start();
            InitState();
        }

        protected abstract void InitState();

        protected override void OnPlayAnim(UnityEngine.Object arg0)
        {
            base.OnPlayAnim(arg0);

            time = 1f / duration;
            coroutine = StartCoroutine(PlayAnim(OnAnimComplete));
        }

        protected override void OnSetInActive(UnityEngine.Object target)
        {
            base.OnSetInActive(target);
            StopActivedCoroutine();
        }
        public override void UnDoChanges(UnityEngine.Object target)
        {
            base.UnDoChanges(target);
            StopActivedCoroutine();
        }

        protected virtual void StopActivedCoroutine() {
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
