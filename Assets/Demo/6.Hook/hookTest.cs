using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using WorldActionSystem;

public class hookTest : MonoBehaviour {
    [SerializeField]
    private ActionGroup group;
    [SerializeField]
    private bool auto;
    private void Start()
    {
        group.LunchActionSystem((steps) =>
        {
            group.RemoteController.StartExecuteCommand(null, auto);
        });
    }
}
