using System;
using System.Collections;
using UnityEngine;

public class ProtoUIPanelStart : MonoBehaviour
{
    public ProtoGameControl gameControl;
    public Animation ani;
    public void Setup()
    {
        ani.Play();
        StartCoroutine(WaitThenStart());
    }

    private IEnumerator WaitThenStart()
    {
        yield return new WaitForSeconds(ani.clip.length);
        gameControl.StartGame();
        gameObject.SetActive(false);
        yield return null;
    }
}

