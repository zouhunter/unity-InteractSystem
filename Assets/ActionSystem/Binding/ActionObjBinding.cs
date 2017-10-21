using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using WorldActionSystem;

[RequireComponent(typeof(ActionObj))]
public class ActionObjBinding : MonoBehaviour {
    protected ActionObj actionObj;
    protected virtual void Awake()
    {
        actionObj = gameObject.GetComponent<ActionObj>();
        actionObj.onBeforeStart.AddListener(OnBeforeActive);
        actionObj.onBeforeComplete.AddListener(OnBeforeComplete);
        actionObj.onBeforeUnDo.AddListener(OnBeforeUnDo);
    }
    protected virtual void OnBeforeActive()
    {
    }
    protected virtual void OnBeforeComplete()
    {
    }
    protected virtual void OnBeforeUnDo()
    {

    }
}
