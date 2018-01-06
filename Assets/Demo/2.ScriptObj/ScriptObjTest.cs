using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using WorldActionSystem;

public class ScriptObjTest : MonoBehaviour
{
    public ActionGroupObj obj;
    [SerializeField]
    private Step[] steps;
    private ActionGroup group;
    private void Start()
    {
        group = InitActionGroup(obj);
        group.LunchActionSystem(steps, (worpSteps) =>
        {
            LoopExecute();
        });
    }
    static ActionGroup InitActionGroup(ActionGroupObj obj)
    {
        var holder = new GameObject("GroupTemp").AddComponent<ActionGroup>();
        holder.groupKey = obj.groupKey;
        holder.totalCommand = obj.totalCommand;
        Utility.CreateRunTimeObjects(holder.transform, obj.prefabList);
        return holder;
    }
    private void LoopExecute()
    {
        Debug.Log("LoopExecute");

        if (group.RemoteController.StartExecuteCommand((x) => { Debug.Log("COMPLETE"); }, false))
        {
            Debug.Log("current:" + group.RemoteController.CurrCommand);
        }
        else
        {
            group.RemoteController.ToAllCommandStart();
            //LoopExecute();
        }
    }
}
