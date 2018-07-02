using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Actions
{
    /// <summary>
    /// 移除一定的范围并添加刚体组件
    /// </summary>

    public class DetachItem : ActionItem
    {
        private Rigidbody m_rigidbody;
        public override bool OperateAble
        {
            get
            {
                return targets.Count == 0;
            }
        }

        private Vector3 startPos;
        private Quaternion startRot;
        public PickUpAbleFeature pickupableFeature = new PickUpAbleFeature();
        public CompleteAbleItemFeature completeAbleFeature = new CompleteAbleItemFeature();
        public const string layer = "i:detachitem";

        protected override void Start()
        {
            base.Start();
            startPos = transform.localPosition;
            startRot = transform.localRotation;
        }
        protected override List<ActionItemFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();

            pickupableFeature.Init(this, layer);
            features.Add(pickupableFeature);

            completeAbleFeature.Init(this, AutoExecute);
            features.Add(completeAbleFeature);

            return features;
        }
        public void AutoExecute(Graph.OperaterNode node)
        {
            OnDetach();
        }

        public override void StepComplete()
        {
            base.StepComplete();
            if (!OperateAble)
            {
                AddRigibody();
            }
        }
        public override void StepUnDo()
        {
            base.StepUnDo();
            RemoveRigibody();
            transform.localPosition = startPos;
            transform.localRotation = startRot;
        }

        internal void OnDetach()
        {
            AddRigibody();
            completeAbleFeature.OnComplete();
        }

        private void AddRigibody()
        {
            if (m_rigidbody == null)
            {
                m_rigidbody = gameObject.AddComponent<Rigidbody>();
            }
        }
        private void RemoveRigibody()
        {
            if (m_rigidbody != null)
            {
                m_rigidbody = gameObject.AddComponent<Rigidbody>();
                Destroy(m_rigidbody);
            }
        }
    }
}