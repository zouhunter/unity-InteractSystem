using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldActionSystem;

public class LoopExecuteBehaiver : MonoBehaviour {
    [SerializeField]
    private bool autoExecute;
    [SerializeField]
    private ActionGroup group;
    [SerializeField]
    private Step[] steps;
 
    private string currentStep;
    public Config config;
	// Use this for initialization
	void Start () {
        Config.Global = config;
        group.LunchActionSystem(steps, (worpedSteps) =>
        {
            Debug.Log("InitOK");
            LoopExecute(true);
        });
        group.onUserError += OnUserError;
    }

    private void OnUserError(string step, string info)
    {
        Debug.Log(step + ":" + info);
    }

    private void LoopExecute(bool haveNext)
    {
        Debug.Log("LoopExecute");

        if(group.RemoteController.StartExecuteCommand(LoopExecute, autoExecute))
        {
            if(group.RemoteController.CurrCommand != null)
            {
                currentStep = group.RemoteController.CurrCommand.StepName;
            }
        }
        else
        {
            Debug.Log("ToAllCommandStart");
            group.RemoteController.ToAllCommandStart();
            LoopExecute(true);
        }
    }
    private void OnGUI()
    {
        GUILayout.Label("[当前步骤：]" + currentStep);
    }
}
