using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProtoGameControl : MonoBehaviour
{
    public CampaignLoader campaignLoader;
    public ProtoUIPanelStart panelStart;
    public GameObject panelComplete;
    public ProtoUIPanelFail panelFail;

    public ProtoAudioClipControl AudioClipControl;

    // Start is called before the first frame update
    void Start()
    {
        ResetUI();
    }

    private void ResetUI()
    {
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
        campaignLoader.LoadCampaign();
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

        AudioClipControl.OnComplete += OnGameComplete;
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
        AudioClipControl.ReplenishMoves(20);
        panelFail.gameObject.SetActive(false);
    }

    private void OnContinueAd()
    {
        AudioClipControl.ReplenishMoves(10);
        panelFail.gameObject.SetActive(false);
    }

    private void OnFailClose()
    {
        panelFail.gameObject.SetActive(false);
        ResetUI();
        StartGame();
    }

    void Exit()
    {
        AudioClipControl.OnComplete -= OnGameComplete;
    }
}
