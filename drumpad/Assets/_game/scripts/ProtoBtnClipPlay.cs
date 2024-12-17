using System;
using UnityEngine;
using UnityEngine.UI;

public class ProtoBtnClipPlay : MonoBehaviour
{
    public AudioSource source;
    public Button btn;

    public bool isClipPlaying = false;

    public float playTimeProgress;

    public event Action onPlay;
    
    private Action<int> onClick;
    private int index;
    private int actualIndex;
    public StructBtnData Data;
    public bool isHovering;

    private GameLevelData gameLevelData;

    public bool IsSet { get; private set; }

    public bool AllowPlayerDragOut
    {
        get { return !Data.locked; }
    }

    public bool IsStarter
    {
        get { return Data.actualIndex == 0; }
    }
    
    public static event Action<StructClipPlayed> OnClipPlayed;
    public static event Action<ProtoBtnClipPlay, float, float> OnSlideIn;
    public static event Action<ProtoBtnClipPlay> OnClipGrabbed;
    public static event Action<ProtoBtnClipPlay> OnClipDropped;
    public static event Action<ProtoBtnClipPlay, bool> OnClipToggleShow;

    void Start()
    {
        btn.onClick.AddListener(ClickPlay);
    }

    void OnEnable()
    {
        OnClipDropped += OnClipDroppedCheckPlaying;
    }

    private void OnClipDroppedCheckPlaying(ProtoBtnClipPlay play)
    {
        if (isClipPlaying)
        {
            Stop();
        }
    }

    private void ClickPlay()
    {
        if (onClick == null)
        {
            return;
        }
        onClick.Invoke(index);
    }

    public void SetLevelData(GameLevelData gameLevelData)
    {
        this.gameLevelData = gameLevelData;
        IsSet = false;

        SlideIn(UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(-100, 100));
    }

    public void SetData(AudioClip clip, StructBtnData structBtnData)
    {
        Data = structBtnData;
        source.clip = clip;
        index = Data.index;
        actualIndex = Data.actualIndex;
        IsSet = true;
        SetEnabled(true);
    }

    public void SlideIn(float delay, float distance)
    {
        OnSlideIn?.Invoke(this, delay, distance);
    }


    public void Drop()
    {
        OnClipDropped?.Invoke(this);
    }

    public void SetAction(Action<int> onClick)
    {
        this.onClick = onClick;
    }

    public void Play()
    {
        PlaySnippet(gameLevelData.clips[Data.actualIndex].startingPoint, gameLevelData.clips[Data.actualIndex].duration);
        isClipPlaying = true;
        if (OnClipPlayed != null)
        {
            OnClipPlayed(new StructClipPlayed()
            {
                index = this.index,
            });
        }
    }
    /// <summary>
    /// Plays a snippet of the assigned audio clip.
    /// </summary>
    /// <param name="startingPoint">The starting point in seconds from where the audio snippet begins.</param>
    /// <param name="duration">The duration of the audio snippet in seconds.</param>
    public void PlaySnippet(float startingPoint, float duration)
    {
        if (source.clip == null)
        {
            Debug.LogError("No AudioClip assigned to the AudioSource.");
            return;
        }

        if (startingPoint < 0 || startingPoint >= source.clip.length)
        {
            Debug.LogError("Starting point is out of bounds.");
            return;
        }

        if (duration <= 0 || startingPoint + duration > source.clip.length)
        {
            Debug.LogError("Invalid duration specified.");
            return;
        }

        StartCoroutine(PlaySnippetCoroutine(startingPoint, duration));
    }

    private System.Collections.IEnumerator PlaySnippetCoroutine(float startingPoint, float duration)
    {
        source.time = startingPoint;
        source.Play();

        yield return new WaitForSeconds(duration);

        Stop();
    }

    public void Stop()
    {
        source.Stop();
        isClipPlaying = false;
    }
    
    void Update()
    {
        // check if source is playing
        if (isClipPlaying)
        {
            if (!source.isPlaying)
            {
                isClipPlaying = false;
            }
            else
            {
                playTimeProgress = (source.time - gameLevelData.clips[Data.actualIndex].startingPoint) / gameLevelData.clips[Data.actualIndex].duration;
            }
        }
    }

    public void SetEnabled(bool e)
    {
        btn.interactable = e;
    }

    public void ShowHide(bool shown = false)
    {
        OnClipToggleShow?.Invoke(this, shown);
    }

    internal void Grab()
    {
        OnClipGrabbed?.Invoke(this);
    }
}

public struct StructClipPlayed
{
    public int index;
}
