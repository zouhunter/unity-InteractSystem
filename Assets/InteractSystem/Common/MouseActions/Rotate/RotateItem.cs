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
        [SerializeField]
        protected ClickAbleFeature clickAbleFeature = new ClickAbleFeature();

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

        [SerializeField, Attributes.CustomField("移动距离")]//旋转的同时向指定方向移动一定的距离
        protected float moveDistence;

        [SerializeField, Attributes.CustomField("轴向标记")]
        private Transform _directionHolder;
        [SerializeField, Attributes.CustomField("旋转对象")]
        private Transform operater;
        public Vector3 Direction { get; private set; }
        public override bool OperateAble
        {
            get
            {
                return targets.Count == 0;
            }
        }
        public Vector3 StartPos { get { return startPos; } }
        private float currAngle;
        private Vector3 startPos;
        private Quaternion startRot;
        private FloatComparer comparer;
        protected const float deviation = 1f;
        protected CompleteAbleItemFeature completeFeature = new CompleteAbleItemFeature();

        public const string layer = "i:rotateitem";

        protected override void Start()
        {
            base.Start();
            InitState();
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
            var target = Direction * triggerAngle + startRot.eulerAngles;
            var start = operater.eulerAngles;

            for (float timer = 0; timer < autoCompleteTime; timer += Time.deltaTime)
            {
                yield return null;
                currAngle = Mathf.Lerp(0, triggerAngle, timer / autoCompleteTime);
                operater.eulerAngles = Vector3.Lerp(start, target, timer / autoCompleteTime);
                RefeshPosition();
            }
            if (Actived)
            {
                completeFeature.OnComplete(firstLock);
            }
        }

        private void InitState()
        {
            Direction = _directionHolder.forward;//右手坐标系?
            startRot = operater.rotation;
            startPos = transform.position;
            if (operater == null) operater = clickAbleFeature.collider.transform;
        }

        protected override void OnSetActive(UnityEngine.Object target)
        {
            base.OnSetActive(target);
            operater.rotation = startRot;
            transform.position = startPos;
            Notice(_directionHolder);
        }


        protected override void OnSetInActive(UnityEngine.Object target)
        {
            base.OnSetInActive(target);
            UnNotice(_directionHolder);
            if (completeMoveBack)
            {
                currAngle = 0;
                operater.rotation = startRot;
                transform.position = startPos;
            }
            else
            {
                currAngle = triggerAngle;
                operater.rotation = Quaternion.Euler(Direction * triggerAngle) * startRot;
                RefeshPosition();
            }
        }
        public override void UnDoChanges(UnityEngine.Object target)
        {
            base.UnDoChanges(target);
            UnNotice(_directionHolder);
            operater.rotation = startRot;
            transform.position = startPos;
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
                        completeFeature.OnComplete(firstLock);
                    }
                }));
        }

        private IEnumerator Clamp(UnityAction onComplete)
        {
            if (currAngle > maxAngle || currAngle < minAngle)
            {
                currAngle = Mathf.Clamp(currAngle, minAngle, maxAngle);
                var target = Quaternion.Euler(Direction * currAngle) * startRot;
                var start = operater.rotation;
                for (float timer = 0; timer < 1f; timer += Time.deltaTime)
                {
                    yield return null;
                    operater.rotation = Quaternion.Lerp(start, target, timer);
                    RefeshPosition();
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
            operater.Rotate(Direction, amount, Space.World);
            RefeshPosition();
        }

        protected void RefeshPosition()
        {
            var distence = (currAngle - 0) / (maxAngle - minAngle) * moveDistence;
            transform.position = startPos + Direction.normalized * distence;
        }
    }
}

