using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Control the ghost of the button that we want to drag and drop
/// </summary>
public class ProtoTileGrab : MonoBehaviour
{
    public ProtoBtnClipDragUI btnClipDragUIProto;
    public ProtoGameControl GameControl;
    
    private ProtoBtnClipDragUI btn;
    private PointerEventData currentData;
    private ProtoBtnClipPlay hoveringBtn;
    
    void Start()
    {
        ProtoBtnClipDragUI.OnDragBegin += OnDragBegin;
        ProtoBtnClipDragUI.OnDragEnd += OnDragEnd;
    }

    private void OnDragBegin(ProtoBtnClipDragUI arg1, PointerEventData arg2)
    {
        btn = arg1;
        currentData = arg2;
        btnClipDragUIProto.btn.SetData(arg1.btn.Data);
    }

    private void OnDragEnd(ProtoBtnClipDragUI arg1, PointerEventData arg2)
    {
        if (hoveringBtn)
        {
            hoveringBtn.Data.isHovering = false;
            // do the swap!
            GameControl.InsertBtnHere(arg1, hoveringBtn);
        }
        
        btn = null;
    }

    void Update()
    {
        if (btn)
        {
            // show the hovering button at the mouse position
            btnClipDragUIProto.gameObject.SetActive(true);
            btnClipDragUIProto.transform.position = currentData.position;

            // Check if the mouse is hovering over any of the buttons (to insert into the correct position)
            if (GameControl)
            {
                foreach (var btn in GameControl.AudioClipControl.btns)
                {
                    btn.Data.isHovering =
                        (RectTransformUtility.RectangleContainsScreenPoint(btn.GetComponent<RectTransform>(),
                            currentData.position));
                    
                    if (btn.Data.isHovering)
                    {
                        hoveringBtn = btn;
                    }
                }
            }
        }
        else
        {
            btnClipDragUIProto.gameObject.SetActive(false);
        }
    }
}
