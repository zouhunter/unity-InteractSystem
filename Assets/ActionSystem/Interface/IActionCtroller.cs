using UnityEngine;
using UnityEngine.Events;
using System.Collections;
namespace WorldActionSystem
{
    public interface IActionCtroller
    {
        IEnumerator Update();
        void InitCommand(ActionCommand trigger);
        void StartExecute(bool forceAuto);
        void EndExecute();
        void UnDoExecute();
    }
}