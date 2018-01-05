using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using WorldActionSystem;

public class openDoor : MonoBehaviour {

    [SerializeField]
    private RotObj obj;
    RotateCtrl rotateCtrl;
    private void Awake()
    {
        rotateCtrl = new RotateCtrl();
        new PickUpController(this);
        obj.OnStartExecute(false);
        obj.onEndExecute = () => { Debug.Log("Complete"); };
    }
    private void Update()
    {
        rotateCtrl.Update();
    }
}
