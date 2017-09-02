using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class OpenUICmdResponce : MonoBehaviour {
    public string stepName;
    public UnityAction<object> onComplete;
    private void OnGUI()
    {
        GUILayout.Space(100);
        if (onComplete != null && GUILayout.Button("结束" + stepName))
        {
           if(onComplete != null) onComplete.Invoke(stepName+"结束了");
        }
    }
}
