using UnityEngine;
using UnityEngine.Events;
using System.Collections;
namespace WorldActionSystem
{
    public interface IActionCtroller
    {
        void Update();
        void OnStartExecute(bool forceAuto);
        void OnEndExecute();
        void OnUnDoExecute();
    }
}