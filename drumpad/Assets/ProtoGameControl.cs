using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoGameControl : MonoBehaviour
{

    public GameObject panelStart;
    public GameObject panelComplete;

    public ProtoAudioClipControl AudioClipControl;

    // Start is called before the first frame update
    void Start()
    {
        panelComplete.SetActive(false);
        panelStart.SetActive(true);
    }

    public void StartGame()
    {
        AudioClipControl.StartGame();

        AudioClipControl.OnComplete += OnGameComplete;
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
