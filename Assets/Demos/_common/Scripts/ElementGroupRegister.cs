using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using InteractSystem;

public class ElementGroupRegister : MonoBehaviour {
    public ElementGroup group;

    private void Awake()
    {
        if (group != null)
        {
            group.SetActive(transform);
        }    
    }
}
