using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
#if !NoFunction
using HighlightingSystem;
#endif
using WorldActionSystem;

public class ShaderHighLight : IHighLightItems
{
#if !NoFunction
    private bool isOn;
    private float freq = 1;
    public Dictionary<Renderer, Highlighter> highlightDic = new Dictionary<Renderer, Highlighter>();
#endif
    public void HighLightTarget(Renderer go, Color color)
    {
        if (!isOn) return;
#if !NoFunction
        Highlighter highlighter;
        if (!highlightDic.ContainsKey(go))
        {
            highlighter = go.gameObject.GetComponent<Highlighter>();
            if(highlighter == null){
                highlighter = go.gameObject.AddComponent<Highlighter>();
            }
            highlighter.On();
            highlighter.SeeThroughOn();
            highlightDic.Add(go, highlighter);
        }
        highlightDic[go].FlashingOn(Color.white, color, freq);
#endif
    }

    public void UnHighLightTarget(Renderer go)
    {
#if !NoFunction
        Highlighter highlighter;
        if (highlightDic.TryGetValue(go, out highlighter))
        {
            highlighter.Off();
        }
#endif
    }

    public void SetState(bool isOpen)
    {
        this.isOn = isOpen;
    }
}
