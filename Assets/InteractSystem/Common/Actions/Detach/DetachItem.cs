using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Common.Actions
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
        public ClickAbleFeature clickAbleFeature = new ClickAbleFeature();
        public CompleteAbleItemFeature completeAbleFeature = new CompleteAbleItemFeature();
        protected override void Start()
        {
            base.Start();
            startPos = transform.localPosition;
            startRot = transform.localRotation;
        }
        protected override List<ActionItemFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();
            clickAbleFeature.target = this;
            clickAbleFeature.LayerName = Layers.detachItemLayer;
            features.Add(clickAbleFeature);
            completeAbleFeature.target = this;
            completeAbleFeature.onAutoExecute = AutoExecute;
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
            completeAbleFeature. OnComplete();
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