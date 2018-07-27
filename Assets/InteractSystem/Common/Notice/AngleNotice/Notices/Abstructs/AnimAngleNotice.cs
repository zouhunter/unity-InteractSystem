using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractSystem.Notice;

namespace InteractSystem.Notice
{
    public abstract class AnimAngleNotice: AngleNotice
    {
        [SerializeField, Range(0.5f, 5f), Attributes.CustomField("动画时间")]
        protected float animTime = 2f;
        protected float animTimer;
        [SerializeField]
        protected AnimationCurve animCurve;

        public override void Update()
        {
            base.Update();
            if (noticed.Count > 0)
            {
                animTimer += Time.deltaTime;
                if (animTimer > animTime)
                {
                    animTimer = 0f;
                }
            }
            if (animTime < 0.02f) return;
            foreach (var item in noticed)
            {
                var angle = angleCtrl[item];
                if (angle)
                {
                    UpdateAngleState(item, angle, animCurve.Evaluate(animTimer / animTime));
                }
            }
        }
        protected abstract void UpdateAngleState(Coordinate target, GameObject angle,float step);
    }

}
