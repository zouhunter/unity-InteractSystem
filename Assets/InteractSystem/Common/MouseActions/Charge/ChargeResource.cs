using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace InteractSystem.Actions
{
    public class ChargeResource : ActionItem
    {
        [SerializeField]
        private ChargeData startData;
        [SerializeField]
        private float _capacity = 1;
        public string type { get { return startData.type; } }
        public float current { get; private set; }
        public ChargeEvent onChange { get; set; }
        public float capacity { get { return _capacity; } }
        public bool Used { get; set; }
        public override bool OperateAble
        {
            get {
                return true;
            }
        }
  
        private ElementController elementCtrl;
        public const string layer = "i:chargeresource";

        protected override void Awake()
        {
            base.Awake();
            elementCtrl = ElementController.Instence;
            elementCtrl.RegistElement(this);
            InitLayer();
        }

        protected override void Start()
        {
            base.Start();
            InitCurrent();
        }
        public override void StepUnDo()
        {
            base.StepUnDo();
            var extro = current - startData.value;
            if (onChange != null)
            {
                //把多的去掉
                onChange.Invoke(transform.position, new ChargeData(type, -extro), null);
            }
            current = startData.value;
        }

        public void Subtruct(float value, UnityAction onComplete)
        {
            current -= value;
            if (onChange != null)
            {
                onChange.Invoke(transform.position, new ChargeData(type, -value), onComplete);
            }
            else
            {
                if (onComplete != null)
                    onComplete.Invoke();
            }
        }

        private void InitCurrent()
        {
            current = startData.value;
            if (onChange != null)
            {
                onChange.Invoke(transform.position, new ChargeData(type, current), null);
            }
        }

        private void InitLayer()
        {
            GetComponentInChildren<Collider>().gameObject.layer = LayerMask.NameToLayer(layer);
        }
    }

}