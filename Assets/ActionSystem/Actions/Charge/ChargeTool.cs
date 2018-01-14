using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace WorldActionSystem
{
    /// <summary>
    /// 用于原料的吸取和填入
    /// </summary>
    public class ChargeTool : PickUpAbleItem, ISupportElement
    {
        [SerializeField]
        private List<string> _supportType;
        [SerializeField]
        private float capacity;

        private Vector3 startPos;
        private ChargeData? chargeData;
        public ChargeEvent onLoad;

        public List<string> supportTypes { get { return _supportType; } }
        public bool charged { get { return chargeData != null; } }
        public ChargeData data { get { return (ChargeData)chargeData; } }
    
        public bool Started { get; private set; }
        private ElementController elementCtrl;

        protected override void Awake()
        {
            base.Awake();
            elementCtrl = ElementController.Instence;
            elementCtrl.RegistElement(this);
        }
        private void OnEnable(){
            startPos = transform.localPosition;
        }
        private void OnDestroy()
        {
            if(elementCtrl != null)
            elementCtrl.RemoveElement(this);
        }
        public override void OnPickDown()
        {
            base.OnPickDown();
            transform.localPosition = startPos;
        }
        public override void OnPickUp()
        {
            base.OnPickUp();
        }
        public override void SetPosition(Vector3 pos)
        {
            transform.position = pos;
        }

        internal bool CanLoad(string type)
        {
            return _supportType.Contains(type);
        }
        /// <summary>
        /// 吸入
        /// </summary>
        /// <param name="chargeResource"></param>
        internal void LoadData(ChargeData data)
        {
            chargeData = data;
            if (onLoad != null)
                onLoad.Invoke(data);
        }
        /// <summary>
        /// 导出
        /// </summary>
        internal void Charge()
        {
            if (onLoad != null)
            {
                var d = new ChargeData(data.type, -data.value);
                onLoad.Invoke(d);
            }
            chargeData = null;
        }

        public void StepActive()
        {
            PickUpAble = true;
        }

        public void StepComplete()
        {
            PickUpAble = false;
        }

        public void StepUnDo()
        {
            PickUpAble = false;
            chargeData = null;
        }
    }
}