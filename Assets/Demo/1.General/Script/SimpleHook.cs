using UnityEngine;
using System.Collections;
using WorldActionSystem;
using System;

public class SimpleHook : ActionHook
{
    protected override bool autoComplete
    {
        get
        {
            return true;
        }
    }

    public override void OnStartExecute(bool auto = false)
    {
        base.OnStartExecute(auto);
        Debug.Log("SimpleHook_Complete:"+name);
    }
}
