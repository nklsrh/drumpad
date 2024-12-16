using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProtoUIPanelLivesLost : MonoBehaviour
{
    public Button btnClose;
    public Button btnRetry;

    private StructPanelLivesLost param;


    void Start()
    {
        btnRetry.onClick.AddListener(OnRetry);   
        btnClose.onClick.AddListener(OnClose);   
    }

    public void Setup(StructPanelLivesLost param)
    {
        this.param = param;

        bool hasLives = true;
        if (CurrencyManager.Instance.GetCurrencyAmount(CurrencyManager.CURRENCY_LIVES) == 0)
        {
            hasLives = false;
        }
        
        btnRetry.gameObject.SetActive(hasLives);
    }

    private void OnRetry()
    {
        param.onRetry?.Invoke();
    }

    private void OnClose()
    {
        param.onClose?.Invoke();
    }

    internal void Show()
    {
        // todo show level number
    }
}

public struct StructPanelLivesLost
{
    public Action onRetry;
    public Action onClose;
}
