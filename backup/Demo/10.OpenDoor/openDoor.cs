using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using WorldActionSystem;
using WorldActionSystem.Actions;
using System;

public class openDoor : MonoBehaviour {

    [SerializeField]
    private RotObj obj;
    RotateCtrl rotateCtrl;
    private void Awake()
    {
        rotateCtrl = new RotateCtrl();
        var pick = new PickUpController(this);
        pick.onPickup += OnPickUpItem;
        obj.OnStartExecute(false);
        obj.onEndExecute = () => { Debug.Log("Complete"); };
    }

    private void OnPickUpItem(PickUpAbleItem arg0)
    {
        Debug.Log("PickedUp:" + arg0);
    }

    private void Update()
    {
        rotateCtrl.Update();
    }
}
