using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class InfoTextShow : MonoBehaviour,IPointerEnterHandler
{
    [SerializeField]
    private int fontSize = 10;
    [SerializeField]
    private bool specialSize = false;
    [SerializeField]
    private Color fontColor = Color.white;
    [SerializeField]
    private bool specialColor = false;
    [SerializeField]
    private string textshow;
    [SerializeField]
    private Vector2 showPos;
    public bool isWorld = true;


    public void Start()
    {
        if (string.IsNullOrEmpty(textshow))
        {
            textshow = name;
        }
        isWorld = !GetComponent<RectTransform>();
    }
    void ShowText(Text textItem)
    {
        Vector3 position = Vector3.zero;

        if (!isWorld)
        {
            position += transform.position;
        }
        else
        {
            position += Camera.main.WorldToScreenPoint(transform.position);
        }

        position.x += showPos.x;
        position.y += showPos.y;

        textItem.transform.position = position;
        if (specialSize) textItem.fontSize = fontSize;
        if (specialColor) textItem.color = fontColor;
        textItem.text = string.IsNullOrEmpty(textshow) ? name : textshow;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
       FindObjectOfType<TextInfoBehiaver>().HandleData((object)new Action<Text>(ShowText));
    }
}
