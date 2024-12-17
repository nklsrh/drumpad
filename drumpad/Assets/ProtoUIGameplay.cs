using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ProtoUIGameplay : MonoBehaviour
{
    public TextMeshProUGUI[] txtLevelText;

    public GameObject playtestOverlay;

    // Start is called before the first frame update
    void OnEnable()
    {
        CampaignProgressManager.OnLoad += OnLoad;
        ProtoAudioClipControl.OnPlayTestSequence += OnPlaytest;
        ProtoAudioClipControl.OnComplete += OnComplete;
    }

    void OnDisable()
    {
        CampaignProgressManager.OnLoad -= OnLoad;
        ProtoAudioClipControl.OnPlayTestSequence -= OnPlaytest;
        ProtoAudioClipControl.OnComplete -= OnComplete;
    }

    void Start()
    {
        playtestOverlay.gameObject.SetActive(false);
    }

    private void OnComplete(bool obj)
    {
        playtestOverlay.GetComponent<CanvasGroup>().DOFade(0.0f, 0.5f).OnComplete(() =>
        {
            playtestOverlay.SetActive(false);
        });
    }

    private void OnPlaytest()
    {
        playtestOverlay.SetActive(true);
        var ca = playtestOverlay.GetComponent<CanvasGroup>();
        ca.alpha = 0;
        ca.DOFade(1.0f, 1f);
    }

    private void OnLoad(CampaignProgress progress)
    {
        foreach (var t in txtLevelText)
        {
            t?.SetText("Level " + progress.GetCurrentLevelIndexString());
        }
    }
}
