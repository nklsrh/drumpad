using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProtoGameControl : MonoBehaviour
{
    public CampaignLoader campaignLoader;
    public GameObject panelStart;
    public GameObject panelComplete;

    public ProtoAudioClipControl AudioClipControl;

    private int level;

    // Start is called before the first frame update
    void Start()
    {
        panelComplete.SetActive(false);
        panelStart.SetActive(true);
    }
    

    public void StartGame()
    {
        campaignLoader.LoadCampaign();
        var level = campaignLoader.StartNextLevel();

        if (level.IsEmpty())
        {
            return;
        }

        AudioClipControl.StartGame(level);

        AudioClipControl.OnComplete += OnGameComplete;
    }

    public void StartNextLevel()
    {
        campaignLoader.CompleteCurrentLevel();
        Start();
        StartGame();
    }

    public void OnGameComplete()
    {
        AudioClipControl.OnComplete -= OnGameComplete;
        
        panelComplete.SetActive(true);
    }

    public void InsertBtnHere(ProtoBtnClipDragUI btn, ProtoBtnClipPlay insertBefore)
    {
        AudioClipControl.InsertBtnHere(btn, insertBefore);
    }
}
