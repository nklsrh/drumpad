using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProtoBtnClipUI : MonoBehaviour
{
    public Image btnImg;
    public TextMeshProUGUI btnText;

    public ProtoBtnClipPlay btn;

    public Color colorStarter;
    public Color colorStandard;
    public float multiplierPlaying = 0.25f;
    
    void Start()
    {
        btn.onPlay += OnPlay;
    }

    void SetColor()
    {
        btnImg.color = btn.isClipPlaying ? Color.white : (btn.IsStarter ? colorStarter : colorStandard);
        btnText.text = btn.isClipPlaying ? "ADD" : (btn.IsStarter ? "START" : "?");
    }
    private void OnPlay()
    {
        SetColor();
    }

    void Update()
    {
        SetColor();
    }
}
