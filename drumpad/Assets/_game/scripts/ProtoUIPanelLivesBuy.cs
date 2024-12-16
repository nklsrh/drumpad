using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProtoUIPanelLivesBuy : MonoBehaviour
{
    public Button btnClose;
    public Button btnBuy;
    public TextMeshProUGUI txtLivesCost;

    private StructPanelLivesBuy param;


    void OnEnable()
    {
        btnClose.onClick.AddListener(OnClose);
        btnBuy.onClick.AddListener(OnBuy);
    }

    void OnDisable()
    {
        btnClose.onClick.RemoveListener(OnClose);
        btnBuy.onClick.RemoveListener(OnBuy);
    }

    public void Setup(StructPanelLivesBuy param)
    {
        this.param = param;

        txtLivesCost.SetText(param.livesCost + "");
    }

    private void OnClose()
    {
        param.onClose?.Invoke();
    }

    private void OnBuy()
    {
        if (CurrencyManager.Instance.SpendCurrency(CurrencyManager.CURRENCY_COINS, param.livesCost))
        {
            CurrencyManager.Instance.AddToCurrency(CurrencyManager.CURRENCY_LIVES, param.liveGain);
        }

        param.onBuy?.Invoke();

        OnClose();
    }

    internal void Show()
    {
        // todo show level number
    }
}

public struct StructPanelLivesBuy
{
    public int livesCost;
    public int liveGain;
    public Action onBuy;
    public Action onClose;
}
