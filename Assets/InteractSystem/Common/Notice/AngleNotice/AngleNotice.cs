using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractSystem.Notice;

namespace InteractSystem.Notice
{
    public class AngleNotice : ActionNotice
    {
        [SerializeField]
        protected GameObject anglePrefab;
        [SerializeField]
        protected AnimationCurve animCurve;
        [SerializeField]
        protected Vector3 startPos;
        [SerializeField]
        protected Vector3 endPos;
        [SerializeField]
        protected float rotateSpeed = 2f;
        [SerializeField,Range(0.5f,5f)]
        protected float animTime = 2f;

        protected AngleCtroller angleCtrl { get { return AngleCtroller.Instence; } }
        protected List<Transform> noticed = new List<Transform>();
        protected float animTimer;

        public override void Notice(Transform target)
        {
            if (target != null)
            {
                angleCtrl.Notice(target, anglePrefab);
                if (!noticed.Contains(target))
                    noticed.Add(target);
            }
        }

        public override void Update()
        {
            base.Update();
            if(noticed.Count > 0)
            {
                animTimer += Time.deltaTime;
                if(animTimer > animTime)
                {
                    animTimer = 0f;
                }
            }
            if (animTime < 0.02f) return;
            foreach (var item in noticed)
            {
                var angle = angleCtrl[item];
                if(angle)
                {
                    UpdateAngleState(item, angle);
                }
            }
        }

        protected virtual void UpdateAngleState(Transform target,GameObject angle)
        {
            var p0 = target.transform.position + startPos;
            var p1 = target.transform.position + endPos;

            var pos = Vector3.Lerp(p0, p1, animCurve.Evaluate( animTimer / animTime));
            angle.transform.position = pos;
            angle.transform.Rotate(endPos - startPos, rotateSpeed);
        }

        public override void UnNotice(Transform target)
        {
            if (target)
            {
                angleCtrl.UnNotice(target);
                if (noticed.Contains(target))
                    noticed.Remove(target);
            }
        }
    }
}
