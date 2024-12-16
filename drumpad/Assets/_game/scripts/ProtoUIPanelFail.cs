using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProtoUIPanelFail : MonoBehaviour
{
    public Button btnContinueUpsell;
    public Button btnContinueAd;
    public Button btnClose;

    private StructPanelFail param;


    void Start()
    {
        btnClose.onClick.AddListener(OnButtonClose);   
        btnContinueAd.onClick.AddListener(OnContinueAd);   
        btnContinueUpsell.onClick.AddListener(OnContinueUpsell);   
    }

    public void Setup(StructPanelFail param)
    {
        this.param = param;
    }

    private void OnContinueUpsell()
    {
        param.onContinueUpsell?.Invoke();
    }

    private void OnContinueAd()
    {
        param.onContinueAd?.Invoke();
    }

    private void OnButtonClose()
    {
        param.onClose?.Invoke();
    }

    internal void Show()
    {
        // todo show level number
    }
}

public struct StructPanelFail
{
    public Action onClose;
    public Action onContinueAd;
    public Action onContinueUpsell;
}
