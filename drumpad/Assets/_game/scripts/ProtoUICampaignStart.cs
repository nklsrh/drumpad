using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProtoUICampaignStart : MonoBehaviour
{
    public Button btnPlay;
    public TextMeshProUGUI txtLevel;

    public TextMeshProUGUI[] txtLevelFuture;
    public ProtoUIPanelLivesBuy panelLivesBuy;

    public CampaignLoader campaignLoader;

    void Start()
    {
        btnPlay.onClick.AddListener(OnPlay);

        panelLivesBuy.gameObject.SetActive(false);

        campaignLoader.LoadCampaign();
    }

    void OnEnable()
    {
        CampaignProgressManager.OnLoad += OnProgressLoaded;
    }

    void OnDisable()
    {
        CampaignProgressManager.OnLoad -= OnProgressLoaded;
    }

    private void OnProgressLoaded(CampaignProgress progress)
    {
        txtLevel.SetText(progress.currentLevelIndex+"");

        for (int i = 0; i < txtLevelFuture.Length; i++)
        {
            txtLevelFuture[i].SetText(progress.currentLevelIndex + i + 1 + "");
        }
    }

    private void OnPlay()
    {
        if (CurrencyManager.Instance.GetCurrencyAmount(CurrencyManager.CURRENCY_LIVES) > 0)
        {
            SceneManager.LoadScene("Game");
        }
        else
        {
            Debug.Log("No Lives Left!!!");

            panelLivesBuy.Setup(new StructPanelLivesBuy
            {
                //onBuy = OnBuyLives,
                onClose = ()=> { panelLivesBuy.gameObject.SetActive(false); },
                liveGain = 20,
                livesCost = 500,
            });
            panelLivesBuy.gameObject.SetActive(true);
        }
    }
}
