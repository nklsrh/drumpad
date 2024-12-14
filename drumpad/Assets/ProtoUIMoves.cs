using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProtoUIMoves : MonoBehaviour
{
    public TextMeshProUGUI txtMoves;

    public ProtoAudioClipControl AudioClipControl;
    void OnEnable()
    {
        AudioClipControl.OnStart += OnStart;
        AudioClipControl.OnMove += OnMove;
    }
    void OnDisable()
    {
        AudioClipControl.OnStart -= OnStart;
        AudioClipControl.OnMove -= OnMove;
    }

    void OnStart(ProtoAudioClipControl control)
    {
        OnMove(control);
    }
    
    void OnMove(ProtoAudioClipControl control)
    {
        var totalMoves = control.gameLevelData.moves;
        var currentMoves = control.movesTaken;

        if (txtMoves)
            txtMoves.SetText((totalMoves - currentMoves).ToString());
    }
}
