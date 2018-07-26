using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using InteractSystem;

public class singleCommand : MonoBehaviour {
    public ActionCommand command;
    private void Start()
    {
        command.SetContext(transform);
        command.RegistComplete((x) => { Debug.Log(x +":Completed"); });
        command.RegistAsOperate((stepName, err) =>
        {
            Debug.Log(err);
        });
    }
    private void OnGUI()
    {
        if(GUILayout.Button("StartCommand"))
        {
            command.StartExecute (false);
        }
        if (GUILayout.Button("StartCommand -auto"))
        {
            command.StartExecute(true);
        }
        if (GUILayout.Button("EndCommand"))
        {
            command.EndExecute();
        }
        if (GUILayout.Button("EndStarted"))
        {
            command.objectCtrl.CompleteStarted();
        }
        if (GUILayout.Button("UnDoCommand"))
        {
            command.UnDoExecute();
        }
    }
}
