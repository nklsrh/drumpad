using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ProtoUIPanelStart : MonoBehaviour
{
    public ProtoGameControl gameControl;
    public TextMeshProUGUI txtLevel;
    public Animation ani;
    public void Setup()
    {
        ani.Play();
        txtLevel?.SetText("Level " + gameControl.campaignLoader.ProgressManager.currentProgress.GetCurrentLevelIndexString());
        DOVirtual.DelayedCall(ani.clip.length, () =>
        {
            gameControl.StartGame();
            gameObject.SetActive(false);
        });
        // StartCoroutine(WaitThenStart());
    }

    private IEnumerator WaitThenStart()
    {
        yield return new WaitForSeconds(ani.clip.length);
        yield return null;
    }
}

