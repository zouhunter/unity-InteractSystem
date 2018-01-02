using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldActionSystem;

public class AutoExecute : MonoBehaviour {
    [SerializeField]
    private ActionGroup group;
    [SerializeField]
    private Step[] steps;
    private string currentStep;
    public Config config;
	// Use this for initialization
	void Start () {
        Config.Defult = config;
        group.LunchActionSystem(steps, (worpedSteps) =>
        {
            Debug.Log("InitOK");
            LoopExecute();
        });
    }
	
    private void LoopExecute()
    {
        Debug.Log("LoopExecute");

        if(group.RemoteController.StartExecuteCommand(LoopExecute, false))
        {
            if(group.RemoteController.CurrCommand != null)
            {
                currentStep = group.RemoteController.CurrCommand.StepName;
            }
        }
        else
        {
            group.RemoteController.ToAllCommandStart();
            LoopExecute();
        }
    }
    private void OnGUI()
    {
        GUILayout.Label("[当前步骤：]" + currentStep);
    }
}
