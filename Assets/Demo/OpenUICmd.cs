using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using WorldActionSystem;

public class OpenUICmd : QueueIDObj
{
    public override void StartExecute(bool forceAuto = true)
    {
        base.StartExecute(forceAuto);
        FindObjectOfType<OpenUICmdResponce>().onComplete = OnReceiveBack;
        FindObjectOfType<OpenUICmdResponce>().stepName = StepName;
    }
    void OnReceiveBack(object data)
    {
        Debug.Log(data);
        FindObjectOfType<OpenUICmdResponce>().onComplete = null;
        EndExecute();
    }
}
