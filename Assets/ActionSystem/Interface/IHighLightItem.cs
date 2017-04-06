using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public interface IHighLightItems
    {
        void HighLightTarget(Renderer go,Color color);
        void UnHighLightTarget(Renderer go);
    }

    public interface IHightLightItem
    {
        Renderer Render { get;}
    }
}
