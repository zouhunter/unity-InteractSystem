﻿using UnityEngine;
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
    public class DetachItem : PickUpAbleElement
    {
        private Rigidbody m_rigidbody;
        public override bool OperateAble
        {
            get
            {
                return targets.Count == 0;
            }
        }
        protected override string LayerName
        {
            get
            {
                return Layers.detachItemLayer;
            }
        }
        private Vector3 startPos;
        private Quaternion startRot;

        protected override void Start()
        {
            base.Start();
            startPos = transform.localPosition;
            startRot = transform.localRotation;
        }
        public override void AutoExecute()
        {
            base.AutoExecute();
            OnDetach();
        }

        public override void StepComplete()
        {
            base.StepComplete();
            if (!OperateAble){
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
            OnComplete();
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

        protected override void OnPickUp()
        {
        }

        protected override void OnPickStay()
        {
        }

        protected override void OnPickDown()
        {
        }
    }
}