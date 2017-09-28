using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using WorldActionSystem;

public class OpenUICmdObj : ActionObj
{
    public string cmdName;
    OpenUICmdResponce responce;

    protected override  void Start()
    {
        base.Start();
        responce = FindObjectOfType<OpenUICmdResponce>();

    }
    public override void OnStartExecute()
    {
        base.OnStartExecute();
        responce.onComplete = OnReceiveBack;
        responce.OpenUI(cmdName);
    }

    void OnReceiveBack(object data)
    {
        responce.onComplete = null;
        OnEndExecute();
    }
    public override void OnUnDoExecute()
    {
        base.OnUnDoExecute();
        responce.onComplete = null;
    }
}
