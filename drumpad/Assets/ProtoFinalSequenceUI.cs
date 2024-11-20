using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoFinalSequenceUI : MonoBehaviour
{
    public ProtoBtnClipPlay[] btns;

    public GameObject panelTesting;

    private int length = 0;
    
    void Start()
    {
        ProtoBtnClipPlay.OnClipPlayed += OnClipPlayed;
        ProtoAudioClipControl.OnAddClipSequence += OnAddClipSequence;
        ProtoAudioClipControl.OnPlayTestSequence += OnPlayTestSequence;
        
        // all btns hidden
        foreach (var btn in btns)
        {
            btn.gameObject.SetActive(false);
        }
    }

    private void OnPlayTestSequence()
    {
        panelTesting.SetActive(true);
    }

    private void OnAddClipSequence(StructAddClipSequence obj)
    {
        Debug.Log("Sequence added clip: " + obj.index);
        
        btns[length].SetData(obj.data);
        // btns[length].SetAction(RemoveClip);
        int i = length;
        btns[length].SetAction(it=>
        {
            int ii = i;
            btns[ii].Play();
        });
        
        btns[length].gameObject.SetActive(true);
        
        length++;
    }

    private void RemoveClip(int arg1)
    {
        Debug.Log("Clip removed: " + arg1);
    }

    private void OnClipPlayed(StructClipPlayed obj)
    {
        // Debug.Log("Clip played: " + obj.index);
    }
}
