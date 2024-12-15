using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProtoBtnClipDragUI : MonoBehaviour, IDragHandler, IPointerDownHandler, IEndDragHandler, IPointerUpHandler
{
    public ProtoBtnClipPlay btn;
    
    public static System.Action<ProtoBtnClipDragUI, PointerEventData> OnDragBegin;
    public static System.Action<ProtoBtnClipDragUI, PointerEventData> OnDragEnd;
    public static System.Action<ProtoBtnClipPlay> onTapped;

    bool isDragging = false;

    PointerEventData latestEventData;
    Vector2 totalDelta = Vector2.zero;
    bool isSendingEventUp = false;
    
    Transform originalParent;

    void Start()
    {
        originalParent = transform.parent;
    }

    public void CancelDrag()
    {
        isDragging = false;
        isSendingEventUp = false;
    }
    

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (onTapped != null)
        {
            onTapped.Invoke(btn);
        }
    }   
    
    public void OnPointerUp(PointerEventData eventData)
    {
        CancelDrag();
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        CancelDrag();

        if (OnDragEnd != null)
        {
            OnDragEnd.Invoke(this, eventData);
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (btn.AllowPlayerDragOut)
        {
            totalDelta += eventData.delta;

            if (!isSendingEventUp && latestEventData != eventData)
            {
                if (!isDragging)
                {
                    isDragging = true;

                    latestEventData = null;
                    totalDelta = Vector2.zero;
                }
            }

            if (OnDragBegin != null)
            {
                OnDragBegin.Invoke(this, eventData);
            }
        }
    }
}
