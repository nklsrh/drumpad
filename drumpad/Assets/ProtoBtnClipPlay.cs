using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProtoBtnClipPlay : MonoBehaviour
{
    public AudioSource source;
    public Button btn;

    public bool isClipPlaying = false;

    public event Action onPlay;
    
    private Action<int> onClick;
    private int index;
    private int actualIndex;
    public StructBtnData Data;
    public bool isHovering;

    public bool AllowPlayerDragOut
    {
        get { return !Data.locked; }
    }

    public bool IsStarter
    {
        get { return Data.actualIndex == 0; }
    }
    
    public static event Action<StructClipPlayed> OnClipPlayed;

    void Start()
    {
        btn.onClick.AddListener(ClickPlay);
    }

    private void ClickPlay()
    {
        if (onClick == null)
        {
            return;
        }
        onClick.Invoke(index);
    }

    public void SetData(StructBtnData structBtnData)
    {
        Data = structBtnData;
        source.clip = Data.clip;
        index = Data.index;
        actualIndex = Data.actualIndex;
        SetEnabled(true);
    }

    public void SetAction(Action<int> onClick)
    {
        this.onClick = onClick;
    }

    public void Play()
    {
        source.Play();
        isClipPlaying = true;
        if (OnClipPlayed != null)
        {
            OnClipPlayed(new StructClipPlayed()
            {
                index = this.index,
            });
        }
    }

    public void Stop()
    {
        source.Stop();
        isClipPlaying = false;
    }
    
    void Update()
    {
        // check if source is playing
        if (isClipPlaying && !source.isPlaying)
        {
            isClipPlaying = false;
        }
    }

    public void SetEnabled(bool e)
    {
        btn.interactable = e;
    }
}

public struct StructClipPlayed
{
    public int index;
}
