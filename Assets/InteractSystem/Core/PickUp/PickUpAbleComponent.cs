using UnityEngine;
using UnityEngine.Events;

namespace InteractSystem
{
    public sealed class PickUpAbleComponent : MonoBehaviour
    {
        private Collider _collider;
        private bool _pickUpAble = true;
        [HideInInspector]
        public UnityEvent onPickUp, onPickDown, onPickStay;
        public event UnityAction<Vector3> onSetPosition;
        public event UnityAction<Vector3> onSetViewForward;
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
            if (_collider){
                _collider.gameObject.layer = LayerMask.NameToLayer(Layers.pickUpElementLayer);
            }
        }

        public void OnPickUp()
        {
           if(onPickUp != null)
                onPickUp.Invoke();
        }
        public void OnPickStay()
        {
            if (onPickUp != null)
                onPickStay.Invoke();
        }
        public void OnPickDown()
        {
            if (onPickUp != null)
                onPickDown.Invoke();
        }
        public void SetPosition(Vector3 pos)
        {
            if(onSetPosition != null)
            {
                onSetPosition.Invoke(pos);
            }
            else
            {
                transform.position = pos;
            }
        }
        public void SetViewForward(Vector3 forward) {
            if(onSetViewForward != null)
            {
                onSetViewForward.Invoke(forward);
            }
            else
            {
                transform.forward = forward;
            }
        }

    
    }
}