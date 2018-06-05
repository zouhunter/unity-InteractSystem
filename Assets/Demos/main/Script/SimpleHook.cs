using UnityEngine;
using System.Collections;
using InteractSystem;
using System;

using InteractSystem.Hooks;

public class SimpleHook :TimerHook
{
    public override void OnStartExecute(bool auto = false)
    {
        base.OnStartExecute(auto);
        Debug.Log("SimpleHook_Complete:"+name);
    }
}
