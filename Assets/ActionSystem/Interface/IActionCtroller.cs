using UnityEngine;
using UnityEngine.Events;
using System.Collections;
namespace WorldActionSystem
{
    public interface IActionCtroller
    {
        IEnumerator Update();
        void OnStartExecute(bool forceAuto);
        void OnEndExecute();
        void OnUnDoExecute();
    }
}