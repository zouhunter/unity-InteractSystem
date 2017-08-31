using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class OutlineHighLight : IHighLightItems
    {
        Dictionary<Renderer, Shader> defultShaders = new Dictionary<Renderer, Shader>();
        private bool isOn;
        Shader normalHighlight;
        float outlineWeight = 0.0045f;

        public OutlineHighLight()
        {
            normalHighlight = Resources.Load<Shader>("OutLightting");
        }

        public void HighLightTarget(Renderer render, Color color)
        {
            if (!isOn) return;
            if (!defultShaders.ContainsKey(render))
            {
                defultShaders.Add(render, render.material.shader);
            }

            render.material.shader = normalHighlight;
            render.material.SetFloat("_Outline", outlineWeight);

            render.material.SetColor("_OutlineColor", color);
        }

        public void SetState(bool isOpen)
        {
            this.isOn = isOpen;
        }

        public void UnHighLightAll()
        {
        }

        public void UnHighLightTarget(Renderer renderer)
        {
            Shader oldShader;
            if (defultShaders.TryGetValue(renderer, out oldShader))
            {
                renderer.material.shader = defultShaders[renderer];
                defultShaders.Remove(renderer);
            }

        }
    }

}
