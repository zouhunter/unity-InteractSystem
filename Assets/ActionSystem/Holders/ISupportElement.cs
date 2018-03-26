using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public interface ISupportElement
    {
        string Name { get; }
        bool IsRuntimeCreated { get; set; }
        bool Active { get; }
        void StepActive();
        void StepComplete();
        void StepUnDo();
    }
}
