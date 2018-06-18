using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using InteractSystem;
public class BindingSetter : MonoBehaviour {
    public InteractSystem.Binding.ActionItemHighlighter actionItemHighLighter;
    private void Awake()
    {
        Config.actionItemBindings.Add(actionItemHighLighter);
    }
}
