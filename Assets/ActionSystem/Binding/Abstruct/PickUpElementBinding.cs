using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using WorldActionSystem;
using System;

[RequireComponent(typeof(PlaceElement))]

public class PickUpElementBinding : MonoBehaviour {
    protected PlaceElement pickUpElement;
    protected virtual void Awake()
    {
        pickUpElement = GetComponent<PlaceElement>();
        pickUpElement.onPickUp.AddListener(OnPickUp);
        pickUpElement.onPickDown.AddListener(OnLayDown);
        pickUpElement.onInstallOkEvent += OnInstallOK;
    }

    protected virtual void OnInstallOK()
    {
        
    }
    protected virtual void OnDestroy()
    {
        pickUpElement.onInstallOkEvent -= OnInstallOK;
    }
    protected virtual void OnPickUp()
    {
      
    }
    protected virtual void OnLayDown()
    {

    }
}
