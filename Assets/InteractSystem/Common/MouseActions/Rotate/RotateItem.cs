using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions.Comparers;

namespace InteractSystem.Actions
{
    public class RotateItem : ActionItem
    {
        [SerializeField, Attributes.CustomField("最小角度（左）")]
        protected float minAngle = -30;
        [SerializeField, Attributes.CustomField("最大角度（右）")]
        protected float maxAngle = 30;
        [SerializeField, Attributes.CustomField("触发角度")]
        protected float triggerAngle = 30;
        [SerializeField, Attributes.CustomField("角度限定")]
        protected bool clampHard;
        [SerializeField, Attributes.CustomField("结束时角度重置")]
        protected bool completeMoveBack;
        [SerializeField, Attributes.CustomField("执行时间（自动状态）")]
        protected float autoCompleteTime = 2f;

        [SerializeField, Attributes.CustomField("轴向标记")]
        private Transform _directionHolder;
        private Transform _operater;
        public Transform Operater { get { return clickAbleFeature.collider.transform; } }
        public Vector3 Direction { get; private set; }
        public override bool OperateAble
        {
            get
            {
                return targets.Count == 0;
            }
        }

        private float currAngle;
        private Quaternion startRot;
        private FloatComparer comparer;
        protected const float deviation = 1f;
        protected CompleteAbleItemFeature completeFeature = new CompleteAbleItemFeature();
        [SerializeField]
        protected ClickAbleFeature clickAbleFeature = new ClickAbleFeature();
        public const string layer = "i:rotateitem";

        protected override void Start()
        {
            base.Start();
            InitDirection();
            comparer = new FloatComparer(deviation);
        }
        protected override List<ActionItemFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();

            completeFeature.Init(this, (graph) => StartCoroutine(AutoRotateTo()));
            features.Add(completeFeature);

            clickAbleFeature.Init(this, layer);
            features.Add(clickAbleFeature);

            return features;
        }

        private IEnumerator AutoRotateTo()
        {
            var target = Quaternion.Euler(Direction * triggerAngle) * startRot;
            var start = Operater.rotation;
            for (float timer = 0; timer < autoCompleteTime; timer += Time.deltaTime)
            {
                yield return null;
                Operater.rotation = Quaternion.Lerp(start, target, timer / autoCompleteTime);
            }
            completeFeature.OnComplete();
        }

        private void InitDirection()
        {
            Direction = (_directionHolder.localPosition).normalized;//右手坐标系?
            startRot = Operater.rotation;
        }

        public override void SetActive(UnityEngine.Object target)
        {
            base.SetActive(target);
            Operater.rotation = startRot;
        }


        public override void SetInActive(UnityEngine.Object target)
        {
            base.SetInActive(target);
            if (completeMoveBack)
            {
                currAngle = 0;
                Operater.rotation = startRot;
            }
            else
            {
                currAngle = triggerAngle;
                Operater.rotation = Quaternion.Euler(Direction * triggerAngle) * startRot;
            }
        }
        public override void UnDoChanges(UnityEngine.Object target)
        {
            base.UnDoChanges(target);
            Operater.rotation = startRot;
            currAngle = 0;
        }

        internal bool TryMarchRot()
        {
            return comparer.Equals(currAngle, triggerAngle);
        }

        public void Clamp()
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(Clamp(() =>
                {
                    if (TryMarchRot())
                    {
                        completeFeature.OnComplete();
                    }
                }));
        }

        private IEnumerator Clamp(UnityAction onComplete)
        {
            if (currAngle > maxAngle || currAngle < minAngle)
            {
                currAngle = Mathf.Clamp(currAngle, minAngle, maxAngle);
                var target = Quaternion.Euler(Direction * currAngle) * startRot;
                var start = Operater.rotation;
                for (float timer = 0; timer < 1f; timer += Time.deltaTime)
                {
                    yield return null;
                    Operater.rotation = Quaternion.Lerp(start, target, timer);
                }
            }
            if (onComplete != null) onComplete.Invoke();
        }

        public void Rotate(float amount)
        {
            currAngle += amount;
            if (clampHard)
            {
                if (currAngle < minAngle || currAngle > maxAngle)
                {
                    currAngle -= amount;
                    return;
                }
            }
            Operater.Rotate(Direction, amount, Space.World);
        }
    }
}

