using System;
using UnityEngine;

public class PageInOut : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 outPos;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        outPos = rectTransform.anchoredPosition;
    }

    public void _In()
    {
        rectTransform.anchoredPosition = Vector2.zero;
    }

    public void _Out()
    {
        rectTransform.anchoredPosition = outPos;
    }
}
