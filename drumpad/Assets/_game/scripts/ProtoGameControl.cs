using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ProtoGameControl : MonoBehaviour
{
    public CampaignLoader campaignLoader;
    public ProtoUIPanelStart panelStart;
    public GameObject panelComplete;
    public ProtoUIPanelFail panelFail;
    public ProtoUIPanelLivesLost panelLivesLost;

    public ProtoAudioClipControl AudioClipControl;

    // Start is called before the first frame update
    void Start()
    {
        ResetUI();
    }

    private void ResetUI()
    {
        panelLivesLost.gameObject.SetActive(false);
        panelComplete.SetActive(false);
        panelStart.gameObject.SetActive(true);
        panelStart.Setup();
        panelFail.Setup(new StructPanelFail
        {
            onClose = OnFailClose,
            onContinueAd = OnContinueAd,
            onContinueUpsell = OnContinueUpsell
        });
    }

    public GameLevelData LoadGameLevelDataFromProgress()
    {
        var campaginOverrideID = ProtoUICampaignStart.StructGameParams.campaignID;
        campaignLoader.LoadCampaign(campaginOverrideID);
        var level = campaignLoader.StartNextLevel();

        if (level.IsEmpty())
        {
            return new GameLevelData();
        }
        return level;
    }

    public void StartGame()
    {
        var level = LoadGameLevelDataFromProgress();

        AudioClipControl.StartGame(level);

        ProtoAudioClipControl.OnComplete += OnGameComplete;
    }

    public void StartNextLevel()
    {
        campaignLoader.CompleteCurrentLevel();
        ResetUI();
        StartGame();
    }

    public void OnGameComplete(bool isWon)
    {      
        if (isWon)
        {
            CurrencyManager.Instance.AddToCurrency(CurrencyManager.CURRENCY_COINS, 15);
            panelComplete.SetActive(true);
        }
        else
        {
            panelFail.gameObject.SetActive(true);
            panelFail.Show();
        }
    }

    public void InsertBtnHere(ProtoBtnClipDragUI btn, ProtoBtnClipPlay insertBefore)
    {
        AudioClipControl.InsertBtnHere(btn, insertBefore);
    }

    private void OnContinueUpsell()
    {
        if (CurrencyManager.Instance.SpendCurrency(CurrencyManager.CURRENCY_COINS, 900))
        {
            AudioClipControl.ReplenishMoves(20);
            panelFail.gameObject.SetActive(false);
        }
        else
        {
            // whattlolllll
            CurrencyManager.Instance.ActivateUnlimitedMode(CurrencyManager.CURRENCY_LIVES, 300);
        }
    }

    private void OnContinueAd()
    {
        AudioClipControl.ReplenishMoves(10);
        panelFail.gameObject.SetActive(false);
    }

    private void OnFailClose()
    {
        panelFail.gameObject.SetActive(false);
        
        CurrencyManager.Instance.SpendCurrency(CurrencyManager.CURRENCY_LIVES, 1);

        panelLivesLost.Setup(new StructPanelLivesLost
        {
            onRetry = Retry,
            onClose = Exit
        });
        panelLivesLost.gameObject.SetActive(true);
        panelLivesLost.Show();
    }

    void Retry()
    {
        ResetUI();
        StartGame();
    }

    void Exit()
    {
        ProtoAudioClipControl.OnComplete -= OnGameComplete;

        SceneManager.LoadScene("MainMenu");
    }
}
