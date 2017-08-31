using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class ColorHighLight :  IHighLightItems{

        Dictionary<Renderer, Color> defultColors = new Dictionary<Renderer, Color>();
        public void HighLightTarget(Renderer render, Color color)
        {
            if (!defultColors.ContainsKey(render))
            {
                defultColors.Add(render, render.material.color);
                render.material.color = color;
            }
        }

        public void SetState(bool isOpen)
        {
            
        }

        public void UnHighLightAll()
        {

        }

        public void UnHighLightTarget(Renderer renderer)
        {
            Color oldColor;
            if (defultColors.TryGetValue(renderer, out oldColor))
            {
                renderer.material.color = defultColors[renderer];
                defultColors.Remove(renderer);
            }
        }
    }

}