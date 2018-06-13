using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using InteractSystem;
public class BindingSetter : MonoBehaviour {
    private void Awake()
    {
        Config.actionItemBindings .Add (typeof(InteractSystem.Binding.ActionItemHighlighter) );
    }
}
