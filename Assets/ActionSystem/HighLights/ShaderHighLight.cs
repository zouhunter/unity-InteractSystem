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
    private static bool isOn = true;
    private float freq = 1;
    public Dictionary<Renderer, Highlighter> highlightDic = new Dictionary<Renderer, Highlighter>();
#endif
    public void HighLightTarget(Renderer go, Color color)
    {
        if (go == null) return;
#if !NoFunction
        if (!isOn) return;
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
        if (go == null) return;
#if !NoFunction
        Highlighter highlighter;
        if (highlightDic.TryGetValue(go, out highlighter))
        {
            highlighter.Off();
        }
#endif
    }

    public static void SetState(bool isOpen)
    {
#if !NoFunction
        ShaderHighLight.isOn = isOpen;
#endif
    }

}
