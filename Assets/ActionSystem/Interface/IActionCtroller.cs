using UnityEngine;
using UnityEngine.Events;
using System.Collections;
namespace WorldActionSystem
{
    public interface IActionCtroller
    {
        ControllerType CtrlType { get; }
        void Update();
    }
}