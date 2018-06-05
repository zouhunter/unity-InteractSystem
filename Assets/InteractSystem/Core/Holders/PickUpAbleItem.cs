using UnityEngine;

namespace InteractSystem
{
    public abstract class PickUpAbleItem : MonoBehaviour
    {
        [SerializeField,Attributes.DefultName]
        private string _name;
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    return name;
                }
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        protected bool _pickUpAble = false;
        public virtual bool PickUpAble { get { return _pickUpAble; } set {_pickUpAble = value; } }
        public virtual void OnPickUp() { }
        public virtual void OnPickStay() { }
        public virtual void OnPickDown() { }
        public abstract void SetPosition(Vector3 pos);
        public virtual void SetViewForward(Vector3 forward) { }
        protected Collider _collider;
        public Collider Collider { get { return _collider; } }
        public static bool log = false;
        protected virtual void Awake()
        {
            _collider = GetComponentInChildren<Collider>();
            if (_collider)
            {
                _collider.gameObject.layer = LayerMask.NameToLayer(Layers.pickUpElementLayer);
            }
        }
        protected virtual void Start() { }
        protected virtual void OnEnable() { }
        protected virtual void Update() { }
        protected virtual void OnDestroy() { }
        protected virtual void OnDisable() { }
    }
}