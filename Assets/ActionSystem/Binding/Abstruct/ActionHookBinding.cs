//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using System.Collections;
//using System.Collections.Generic;
//using WorldActionSystem;

//[RequireComponent(typeof(ActionHook))]
//public class ActionHookBinding : MonoBehaviour {
//    protected ActionHook actionObj;
//    protected virtual void Awake()
//    {
//        actionObj = gameObject.GetComponent<ActionHook>();
//        actionObj.onBeforeStart.AddListener(OnBeforeActive);
//        actionObj.onBeforeComplete.AddListener(OnBeforeComplete);
//        actionObj.onBeforeUnDo.AddListener(OnBeforeUnDo);
//    }
//    private void OnDestroy()
//    {
//        if (actionObj)
//        {
//            actionObj.onBeforeStart.RemoveListener(OnBeforeActive);
//            actionObj.onBeforeComplete.RemoveListener(OnBeforeComplete);
//            actionObj.onBeforeUnDo.RemoveListener(OnBeforeUnDo);
//        }
//    }
//    protected virtual void OnBeforeActive(bool forceAuto) { }
//    protected virtual void OnBeforeComplete(bool force) { }
//    protected virtual void OnBeforeUnDo() { }
//}
