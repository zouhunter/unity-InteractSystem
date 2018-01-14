using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class ChargeResource : MonoBehaviour, ISupportElement
    {
        [SerializeField]
        private ChargeData startData;
        public string type { get { return startData.type; } }
        public float current { get; private set; }
        public ChargeEvent onChange;

        #region ISupportElement
        public string Name { get { return name; } }
        public bool Started { get; private set; }

        public void StepActive()
        {
            
        }

        public void StepComplete()
        {
        }

        public void StepUnDo()
        {

        }

        #endregion
        private ElementController elementCtrl;

        protected void Awake()
        {
            elementCtrl = ElementController.Instence;
            elementCtrl.RegistElement(this);
            InitLayer();
        }
        private void OnDestroy()
        {
            if (elementCtrl != null)
                elementCtrl.RemoveElement(this);
        }
        private void Start()
        {
            InitCurrent();
        }
      
        public void Subtruct(float value)
        {
            current -= value;
            onChange.Invoke(new ChargeData(type,-value));
        }
        private void InitCurrent()
        {
            current = startData.value;
            onChange.Invoke(new ChargeData(type, current));
        }
        private void InitLayer()
        {
            GetComponentInChildren<Collider>().gameObject.layer = LayerMask.NameToLayer(Layers.chargeResourceLayer);
        }
    }

}