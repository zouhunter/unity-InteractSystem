using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using InteractSystem;

public class ElementCreater : MonoBehaviour {
    [SerializeField]
    private InputField m_name;
    [SerializeField]
    private Button m_create;

    private void Awake()
    {
        m_create.onClick.AddListener(CreateElementByName);
    }

    private void CreateElementByName()
    {
        var elementName = m_name.text;
        if (!string.IsNullOrEmpty(elementName))
        {
            ElementController.Instence.TryCreateElement<ISupportElement>(elementName, ActionSystem.Instence.transform);
        }
    }
}
