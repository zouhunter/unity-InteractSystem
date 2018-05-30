using UnityEngine;
using System.Collections;
using WorldActionSystem;
using System;

using WorldActionSystem.Hooks;

public class SimpleHook :TimerHook
{
    public override void OnStartExecute(bool auto = false)
    {
        base.OnStartExecute(auto);
        Debug.Log("SimpleHook_Complete:"+name);
    }
}
