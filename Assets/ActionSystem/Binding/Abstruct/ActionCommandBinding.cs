using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using WorldActionSystem;

[RequireComponent(typeof(ActionCommand))]
public class ActionCommandBinding : MonoBehaviour {
    protected ActionCommand cmd;
    protected EventController eventCtrl { get; set; }

    protected virtual void Awake()
    {
        cmd = gameObject.GetComponent<ActionCommand>();
        cmd.onBeforeActive.AddListener(OnBeforeActive);
        cmd.onBeforePlayEnd.AddListener(OnBeforePlayEnd);
        cmd.onBeforeUnDo.AddListener(OnBeforePlayEnd);
    }
    protected virtual void OnDestroy()
    {
        if (cmd)
        {
            cmd.onBeforeActive.RemoveListener(OnBeforeActive);
            cmd.onBeforePlayEnd.RemoveListener(OnBeforePlayEnd);
            cmd.onBeforeUnDo.RemoveListener(OnBeforeUnDo);
        }
    }

    protected virtual void OnBeforeActive(string step)
    {
    }
    protected virtual void OnBeforePlayEnd(string step)
    {
    }
    protected virtual void OnBeforeUnDo(string step)
    {

    }

}
