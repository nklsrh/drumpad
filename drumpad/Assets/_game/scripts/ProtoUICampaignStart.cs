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

    public string campaignID = "campaign-1";
    public static StructGameParams StructGameParams;

    void Start()
    {
        btnPlay.onClick.AddListener(OnPlay);

        panelLivesBuy.gameObject.SetActive(false);

        StructGameParams.campaignID = campaignID;

        campaignLoader.LoadCampaign(campaignID);
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
        txtLevel.SetText(progress.GetCurrentLevelIndexString());

        for (int i = 0; i < txtLevelFuture.Length; i++)
        {
            txtLevelFuture[i].SetText((progress.currentLevelIndex + i + 2) + "");
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

[Serializable]
public struct StructGameParams
{
    public string campaignID;
}