using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using WorldActionSystem;

public class singleObj : MonoBehaviour {
    public ActionObj obj;
    private DragCtrl dragCtrl;
    public bool autoExecute;
    private void Start()
    {
        obj.onBeforeComplete.AddListener((x) => { Debug.Log("complete"); });
        dragCtrl = new DragCtrl();
    }
    private void Update()
    {
        dragCtrl.Update();
    }
    private void OnGUI()
    {
        if (GUILayout.Button("Start Action"))
        {
            obj.OnStartExecute(autoExecute);
        }
        if (GUILayout.Button("End Action"))
        {
            obj.OnEndExecute(true);
        }
        if (GUILayout.Button("UnDo Action"))
        {
            obj.OnUnDoExecute();
        }
    }
}
