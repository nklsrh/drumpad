using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProtoGameControl : MonoBehaviour
{
    public CampaignLoader campaignLoader;
    public ProtoUIPanelStart panelStart;
    public GameObject panelComplete;
    public GameObject panelFail;

    public ProtoAudioClipControl AudioClipControl;

    // Start is called before the first frame update
    void Start()
    {
        panelComplete.SetActive(false);
        panelStart.gameObject.SetActive(true);
        panelStart.Setup();
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
        Start();
        StartGame();
    }

    public void OnGameComplete(bool isWon)
    {
        AudioClipControl.OnComplete -= OnGameComplete;
        
        if (isWon)
        {
            panelComplete.SetActive(true);
        }
        else
        {
            panelFail.SetActive(true);
        }
    }

    public void InsertBtnHere(ProtoBtnClipDragUI btn, ProtoBtnClipPlay insertBefore)
    {
        AudioClipControl.InsertBtnHere(btn, insertBefore);
    }
}
