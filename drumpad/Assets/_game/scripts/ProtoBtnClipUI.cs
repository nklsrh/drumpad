using System;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProtoBtnClipUI : MonoBehaviour
{
    public Image btnImg;
    public Image iconImg;
    public Image inCorrectPositionImg;
    public Image inWrongPositionImg;
    public TextMeshProUGUI btnText;

    public ProtoBtnClipPlay btn;

    public Color colorStarter;
    public Color colorStandard;
    public float multiplierPlaying = 0.25f;
    public Image progress;

    public Sprite[] spriteOptions;
    public Sprite[] iconOptions;

    public Sprite spriteStartBg;
    public Sprite spriteStartIcon;

    Tween slidingTween;

    int assignedTileImageIndex;
    int bgIndex;
    int iconIndex;
    
    void OnEnable()
    {
        btn.onPlay += OnPlay;
        ProtoBtnClipPlay.OnSlideIn += OnSlideIn;
        ProtoBtnClipPlay.OnClipGrabbed += OnGrabbed;
        ProtoBtnClipPlay.OnClipDropped += OnDropped;
        ProtoBtnClipPlay.OnClipToggleShow += OnToggleShow;
        ProtoBtnClipPlay.OnClipToggleCorrect += OnClipToggleCorrect;
    }

    void OnDisable()
    {
        btn.onPlay += OnPlay;
        ProtoBtnClipPlay.OnSlideIn -= OnSlideIn;
        ProtoBtnClipPlay.OnClipGrabbed -= OnGrabbed;
        ProtoBtnClipPlay.OnClipDropped -= OnDropped;
        ProtoBtnClipPlay.OnClipToggleShow -= OnToggleShow;
        ProtoBtnClipPlay.OnClipToggleCorrect -= OnClipToggleCorrect;
    }

    public static void GenerateUniqueCombination(
        int assignedRandomIndex,
        int backgroundColorCount,
        int iconCount,
        out int backgroundColorIndex,
        out int iconIndex)
    {
        int totalCombinations = backgroundColorCount * iconCount;

        // Ensure assignedRandomIndex wraps around the total combinations
        int uniqueIndex = assignedRandomIndex % totalCombinations;

        // Calculate backgroundColorIndex and iconIndex using uniqueIndex
        backgroundColorIndex = uniqueIndex % backgroundColorCount;
        iconIndex = uniqueIndex / backgroundColorCount; // Integer division
    }

    void SetColor()
    {
        if (assignedTileImageIndex != btn.Data.assignedTileImageIndex)
        {
            assignedTileImageIndex = btn.Data.assignedTileImageIndex;

            if (btn.IsStarter)
            {
                btnImg.sprite = spriteStartBg;
                iconImg.sprite = spriteStartIcon;
            }
            else if (btn.Data.locked)
            {
                btnImg.sprite = spriteStartBg;
                iconImg.sprite = spriteStartIcon;
            }
            else
            {
                GenerateUniqueCombination(btn.Data.actualIndex, spriteOptions.Length, iconOptions.Length, out bgIndex, out iconIndex);

                btnImg.sprite = spriteOptions[bgIndex];
                iconImg.sprite = iconOptions[iconIndex];
            }
        }

        // btnImg.color = btn.isHovering ? Color.black : (btn.isClipPlaying ? Color.white : (btn.IsStarter ? colorStarter : colorStandard));

        // btnText.text = btn.isClipPlaying ? "||" : (btn.IsStarter ? "START>" : "");
        iconImg.color = btn.isClipPlaying ? new Color(0.5f, 0.5f, 0.5f, 0.5f) : new Color(1, 1, 1, 0.5f);

        if (!slidingTween.IsActive())
        {
            if (btn.isHovering)
            {
                var l = btnImg.transform.localPosition;
                btnImg.transform.localPosition = Vector3.Lerp(l, new Vector3(40, l.y, l.z), 10 * Time.deltaTime);
                btnImg.color = Color.black * 0.5f;
            }
            else
            {
                btnImg.transform.localPosition = Vector3.zero;
                btnImg.color = Color.white;
            }
        }
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
            slidingTween = btnImg.transform.DOLocalMoveX(0, 1f).SetEase(Ease.OutCirc);
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
        btnImg.transform.DOScale(Vector3.one * 1f, 0.5f).SetEase(Ease.OutElastic);
        btnImg.transform.localPosition = new Vector3(-40, 0, 0);
        btnImg.transform.DOLocalMoveX(0, 0.5f);
    }

    private void OnToggleShow(ProtoBtnClipPlay play, bool shown)
    {
        if (play != this.btn) return;

        btnImg.gameObject.SetActive(shown);
    }

    private void OnClipToggleCorrect(ProtoBtnClipPlay play, bool shown)
    {
        if (play != this.btn) return;

        var correct = play.Data.actualIndex == play.Data.index;
        inCorrectPositionImg?.gameObject.SetActive(shown && correct);
        inWrongPositionImg?.gameObject.SetActive(shown && !correct);
    }

    void Update()
    {
        SetColor();

        if (progress)
        {
            progress.gameObject.SetActive(btn.isClipPlaying);
            progress.fillAmount = btn.playTimeProgress;
        }
    }
}
