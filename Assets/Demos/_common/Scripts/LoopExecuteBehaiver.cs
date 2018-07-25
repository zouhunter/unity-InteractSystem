using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractSystem;

public class LoopExecuteBehaiver : MonoBehaviour
{
    [SerializeField]
    private bool autoExecute;
    [SerializeField]
    private ActionGroup group;
    [SerializeField]
    private string[] steps;

    private string currentStep;
    public Config config;
    private ICommandController ctrl;
    // Use this for initialization
    void Start()
    {
        Config.SetConfig(config);
        string[] worpedSteps;
        ctrl = group.LunchActionSystem(steps, out worpedSteps);
        LoopExecute(true);
        group.EventTransfer.onUserError += OnUserError;
    }

    private void OnUserError(string step, string info)
    {
        Debug.Log(step + ":" + info);
    }

    private void LoopExecute(bool haveNext)
    {
        Debug.Log("LoopExecute");

        if (group.RemoteController.StartExecuteCommand(LoopExecute, autoExecute))
        {
            if (group.RemoteController.CurrCommand != null)
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


        if (GUILayout.Button("EndCommand"))
        {
            group.RemoteController.EndExecuteCommand();
        }

        if (GUILayout.Button("UnDoCommand"))
        {
            var haveLast = group.RemoteController.UnDoCommand();
            if (haveLast)
            {
                group.RemoteController.UnDoCommand();
            }
            LoopExecute(true);
        }
    }
}
