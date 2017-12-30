using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TextInfoBehiaver : MonoBehaviour{
    [SerializeField]
    private Text text;
    private GameObject textBody;
    private List<RaycastResult> result = new List<RaycastResult>();
    private PointerEventData eventData;
    private InfoTextShow lastSelected;
    private Action<Text> showAction;
    void Start()
    {
        textBody = text.gameObject;
        eventData = new PointerEventData(EventSystem.current);
    }

    public void HandleData(object data)
    {
        if (data is Action<Text>) {
            showAction = (Action<Text>)data;
        }
    }

    public void Update()
    {
        eventData.position = Input.mousePosition;
        EventSystem.current.RaycastAll(eventData, result);
        for (int i = 0; i < result.Count; i++)
        {
            InfoTextShow target = result[i].gameObject.GetComponent<InfoTextShow>();
            if (target != null )
            {
                if (target != lastSelected)
                {
                    lastSelected = target;
                    ShowText();
                }
                break;
            }
        }

    }

    private void ShowText()
    {
        textBody.SetActive(true);
        if (showAction != null)
        {
            showAction(text);
        }
    }
}
