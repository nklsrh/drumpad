using System;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
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
    
    void OnEnable()
    {
        btn.onPlay += OnPlay;
        ProtoBtnClipPlay.OnSlideIn += OnSlideIn;
        ProtoBtnClipPlay.OnClipGrabbed += OnGrabbed;
        ProtoBtnClipPlay.OnClipDropped += OnDropped;
    }

    void OnDisable()
    {
        btn.onPlay += OnPlay;
        ProtoBtnClipPlay.OnSlideIn -= OnSlideIn;
        ProtoBtnClipPlay.OnClipGrabbed -= OnGrabbed;
        ProtoBtnClipPlay.OnClipDropped -= OnDropped;
    }

    void SetColor()
    {
        btnImg.color = btn.isHovering ? Color.black : (btn.isClipPlaying ? Color.white : (btn.IsStarter ? colorStarter : colorStandard));
        btnText.text = btn.isClipPlaying ? "||" : (btn.IsStarter ? "START>" : btn.Data.assignedTileImageIndex + "");
    }
    private void OnPlay()
    {
        SetColor();
    }

    private void OnSlideIn(ProtoBtnClipPlay btn, float delay, float distance)
    {
        if (this.btn != btn) return;

        DOVirtual.DelayedCall(delay, () =>
        {
            btnImg.transform.localPosition = new Vector3(btnImg.transform.localPosition.x + distance, btnImg.transform.localPosition.y, btnImg.transform.localPosition.z);
            btnImg.transform.DOLocalMoveX(0, 1f).SetEase(Ease.OutElastic);
        });
    }

    private void OnGrabbed(ProtoBtnClipPlay btn)
    {
        if (this.btn != btn) return;

        btnImg.transform.DOShakeRotation(2f, 2).SetEase(Ease.InOutCubic).SetLoops(-1);
    }

    private void OnDropped(ProtoBtnClipPlay btn)
    {
        if (this.btn != btn) return;

        btnImg.transform.localScale = Vector3.one * 0.4f;
        btnImg.transform.DOScale(Vector3.one * 1f, 0.5f).SetEase(Ease.OutCirc);
    }

    void Update()
    {
        SetColor();
    }
}
