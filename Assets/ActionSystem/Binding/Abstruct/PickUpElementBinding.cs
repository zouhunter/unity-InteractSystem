using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using WorldActionSystem;

[RequireComponent(typeof(PickUpAbleElement))]

public class PickUpElementBinding : MonoBehaviour {
    protected PickUpAbleElement pickUpElement;
    protected virtual void Awake()
    {
        pickUpElement = GetComponent<PickUpAbleElement>();
        pickUpElement.onPickUp.AddListener(OnPickUp);
        pickUpElement.onnLayDown.AddListener(OnLayDown);
    }
    protected virtual void OnPickUp()
    {
      
    }
    protected virtual void OnLayDown()
    {

    }
}
