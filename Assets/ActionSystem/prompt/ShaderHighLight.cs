using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using HighlightingSystem;
using WorldActionSystem;

public class ShaderHighLight : IHighLightItems
{
    private float freq = 1;
    public Dictionary<Renderer, Highlighter> highlightDic = new Dictionary<Renderer, Highlighter>();

    public void HighLightTarget(Renderer go, Color color)
    {
        Highlighter highlighter;
        if (!highlightDic.ContainsKey(go))
        {
            highlighter = go.gameObject.AddComponent<Highlighter>();
            highlighter.On();
            highlighter.SeeThroughOn();
            highlightDic.Add(go, highlighter);
        }
        highlightDic[go].FlashingOn(Color.white, color, freq);
    }

    public void UnHighLightTarget(Renderer go)
    {
        Highlighter highlighter;
        if (highlightDic.TryGetValue(go, out highlighter))
        {
            highlighter.Off();
        }
    }
}
