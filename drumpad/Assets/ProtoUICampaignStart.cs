using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProtoUICampaignStart : MonoBehaviour
{
    public Button btnPlay;
    public TextMeshProUGUI txtLevel;

    public CampaignLoader campaignLoader;

    void Start()
    {
        btnPlay.onClick.AddListener(OnPlay);
    }

    private void OnPlay()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
