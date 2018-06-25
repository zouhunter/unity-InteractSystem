using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions.Comparers;

namespace InteractSystem.Common.Actions
{
    [AddComponentMenu(MenuName.RotObj)]
    public class RotateItem : ActionItem
    {
        public float minAngle = -30;
        public float maxAngle = 30;
        public float triggerAngle = 30;
        public bool clampHard;
        public bool completeMoveBack;
        public float autoCompleteTime = 2f;

        [SerializeField]
        private Transform _directionHolder;
        [SerializeField]
        private Transform _operater;
        public Transform Operater { get { if (_operater == null) _operater = transform; return _operater; } }
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

            clickAbleFeature.Init(this, Layers.rotateItemLayer);
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

        public override void StepActive()
        {
            base.StepActive();
            Operater.rotation = startRot;
        }

     
        public override void StepComplete()
        {
            base.StepComplete();
            if (completeMoveBack)
            {
                Operater.rotation = startRot;
            }
        }
        public override void StepUnDo()
        {
            base.StepUnDo();
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
                StartCoroutine(Clamp(() => {
                    if (TryMarchRot()){
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
            if(clampHard)
            {
                if(currAngle < minAngle || currAngle > maxAngle)
                {
                    currAngle -= amount;
                    return;
                }
            }
            Operater.Rotate(Direction, amount, Space.World);
        }
    }
}

