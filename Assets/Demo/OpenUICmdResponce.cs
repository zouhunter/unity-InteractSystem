using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class OpenUICmdResponce : MonoBehaviour {
    private string commandName;
    public UnityAction<object> onComplete;
    private void OnGUI()
    {
        GUILayout.Space(100);
        if (onComplete != null && GUILayout.Button("结束" + commandName))
        {
           if(onComplete != null)
                onComplete.Invoke(commandName+"结束了");
        }
    }

    internal void OpenUI(string cmdName)
    {
        this.commandName = cmdName;
    }
}
