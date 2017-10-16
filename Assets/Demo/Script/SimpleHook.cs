using UnityEngine;
using System.Collections;
using WorldActionSystem;

public class SimpleHook : ActionHook
{
    public override void OnStartExecute(bool auto = false)
    {
        base.OnStartExecute(auto);
        Invoke("OnEndExecute", 1);
        Debug.Log("SimpleHook_Complete:"+name);
    }
}
