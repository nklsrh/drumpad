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
    
    void OnEnable()
    {
        ProtoBtnClipDragUI.OnDragBegin += OnDragBegin;
        ProtoBtnClipDragUI.OnDragEnd += OnDragEnd;
    }

    void OnDisable()
    {
        ProtoBtnClipDragUI.OnDragBegin -= OnDragBegin;
        ProtoBtnClipDragUI.OnDragEnd -= OnDragEnd;
    }

    private void OnDragBegin(ProtoBtnClipDragUI arg1, PointerEventData arg2)
    {
        if (!arg1.btn.IsSet) return;

        btn = arg1;
        btn.btn.ShowHide(false);

        currentData = arg2;
        btnClipDragUIProto.btn.SetData(GameControl.AudioClipControl.audioClip, arg1.btn.Data);
        btnClipDragUIProto.btn.Grab();
    }

    private void OnDragEnd(ProtoBtnClipDragUI arg1, PointerEventData arg2)
    {
        if (hoveringBtn && arg1.btn != hoveringBtn && hoveringBtn.isActiveAndEnabled && arg1.isActiveAndEnabled)
        {
            hoveringBtn.isHovering = false;
            // do the swap!
            if (hoveringBtn.AllowPlayerDragOut)
                GameControl.InsertBtnHere(arg1, hoveringBtn);
        }
        
        if (btn)
        {
            btn.btn.isHovering = false;
            btn.btn.ShowHide(true);
        }

        btn = null;
        hoveringBtn = null;
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
                bool anyHover = false;
                foreach (var b in GameControl.AudioClipControl.btns)
                {
                    if (b == btn) continue;

                    if (b.AllowPlayerDragOut && b.isActiveAndEnabled)
                    {
                        b.isHovering = (RectTransformUtility.RectangleContainsScreenPoint(b.GetComponent<RectTransform>(),
                            currentData.position));
                    }
                    else
                    {
                        b.isHovering = false;
                    }
                    
                    if (b.isHovering)
                    {
                        hoveringBtn = b;
                        anyHover = true;
                    }
                }
                if (!anyHover)
                {
                    hoveringBtn = null;
                }
            }
        }
        else
        {
            btnClipDragUIProto.gameObject.SetActive(false);
        }
    }
}
