using UnityEngine;
using UnityEngine.Events;

namespace InteractSystem
{
    public sealed class PickUpAbleItem : MonoBehaviour
    {
        private Collider _collider;
        private bool _pickUpAble = true;
        [HideInInspector]
        public UnityEvent onPickUp, onPickDown, onPickStay;
        public static bool log = false;
        public Collider Collider { get { return _collider; } }

        public bool PickUpAble
        {
            get
            {
                return _pickUpAble;
            }
            set
            {
                _pickUpAble = value;
            }
        }

        private void Awake()
        {
            _collider = GetComponentInChildren<Collider>();
            if (_collider)
            {
                _collider.gameObject.layer = LayerMask.NameToLayer(Layers.pickUpElementLayer);
            }
        }

        public void OnPickUp()
        {
            onPickUp.Invoke();
        }
        public void OnPickStay()
        {
            onPickStay.Invoke();
        }
        public void OnPickDown()
        {
            onPickDown.Invoke();
        }
        public void SetPosition(Vector3 pos)
        {

        }
        public void SetViewForward(Vector3 forward) { }

    
    }
}