using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public RectTransform rect;
    public RectTransform rect01;
    public RectTransform rect02;

    private Vector2 oriPos = new Vector2(-320, -200);
    private Vector2 oriSize = new Vector2(480, 270);

    private Vector2 zoomPos = new Vector2(-960, -540);
    private Vector2 zoomSize = new Vector2(1440, 810);

    private bool isOpen;

    private void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            Switch();
        }
    }

    private void Switch()
    {
        isOpen = !isOpen;
        if(isOpen)
        {
            rect.anchoredPosition = zoomPos;
            rect01.sizeDelta = zoomSize;
            rect02.sizeDelta = zoomSize + Vector2.one * 50;
        }
        else
        {
            rect.anchoredPosition = oriPos;
            rect01.sizeDelta = oriSize;
            rect02.sizeDelta = oriSize + Vector2.one * 50;
        }
    }
}
