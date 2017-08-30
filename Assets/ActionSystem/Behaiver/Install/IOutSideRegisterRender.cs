using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public interface IOutSideRegisterRender
    {
        void RegisterRenderer(Renderer renderer);
    }
}
