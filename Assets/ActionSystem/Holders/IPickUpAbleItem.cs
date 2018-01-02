using UnityEngine;

namespace WorldActionSystem
{
    public interface IPickUpAbleItem
    {
        string Name { get; }
        bool PickUpAble { get; set; }
        void OnPickUp();
        void OnPickDown();
        void SetPosition(Vector3 pos);
        Collider Collider { get; }
    }
}