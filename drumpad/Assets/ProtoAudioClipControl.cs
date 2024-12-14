using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProtoAudioClipControl : MonoBehaviour
{
    [HideInInspector]
    public GameLevelData gameLevelData;
    public ProtoBtnClipPlay[] btns;

    [HideInInspector]
    public AudioClip audioClip;
    public AudioSource finishSource;

    private int indexPlaying = -1;
    private bool isPlaying;
    
    public static event Action<StructAddClipSequence> OnAddClipSequence;
    public static event Action OnPlayTestSequence;
    private List<StructBtnData> sequence = new List<StructBtnData>();

    public event Action OnComplete;

    public void StartGame(GameLevelData gameLevelData)
    {
        this.gameLevelData = gameLevelData;

        StartCoroutine(WaitThenStart());
    }

    private IEnumerator WaitThenStart()
    {
        LoadLevelData(gameLevelData);

        yield return new WaitForSeconds(0.5f);
        
        // make new int that randomises values in clips
        int[] randomIndex = new int[gameLevelData.clips.Count];
        for (int i = 0; i < randomIndex.Length; i++)
        {
            randomIndex[i] = i;
        }

        // dont randomise the first tile
        for (int i = 1; i < randomIndex.Length; i++)
        {
            int temp = randomIndex[i];
            int randomIndexValue = UnityEngine.Random.Range(i, randomIndex.Length);
            randomIndex[i] = randomIndex[randomIndexValue];
            randomIndex[randomIndexValue] = temp;
        }
        
        for (int i = 0; i < randomIndex.Length; i++)
        {
            int randomIndexValue = randomIndex[i];
            AddClipToSequence(randomIndexValue);
        }

        SetButtons();
    }

    private void LoadLevelData(GameLevelData gameLevelData)
    {
        audioClip = gameLevelData.GetAudioClip();

        for (int i = 0; i < gameLevelData.clips.Count; i++)
        {
            var c = gameLevelData.clips[i];
            if (i == 0)
            {
                c.startingPoint = gameLevelData.startingPoint;
                gameLevelData.clips[i] = c;
            }
            else
            {
                c.startingPoint = 
                gameLevelData.clips[i - 1].startingPoint + gameLevelData.clips[i - 1].duration;
                gameLevelData.clips[i] = c;
            }
        }
    }

    void SetButtons()
    {
        for (int i = 0; i < btns.Length; i++)
        {
            if (sequence.Count > i)
            {
                btns[i].gameObject.SetActive(true);
                btns[i].SetLevelData(gameLevelData);
                btns[i].SetData(audioClip, sequence[i]);
                btns[i].SetAction(PlaySound);
            }
            else
            {
                btns[i].gameObject.SetActive(false);
            }
        }
    }

    private void PlaySound(int index)
    {
        if (isPlaying)
        {
            btns[indexPlaying].Stop();
        }

        btns[index].Play();
        indexPlaying = index;
        isPlaying = true;
    }

    private void AddClipToSequence(int randomIndex)
    {
        bool shouldFreezeIfFirst = true;

        sequence.Add(new StructBtnData()
        {
            index = sequence.Count,
            locked
             = shouldFreezeIfFirst && sequence.Count == 0,
            actualIndex = randomIndex,
        });
    }

    void CheckComplete()
    {
        if (sequence.Count == gameLevelData.clips.Count)
        {
            bool isCorrect = true;
            // check each element of btns has an Data value that is increasing
            for (int j = 0; j < gameLevelData.clips.Count; j++)
            {
                if (sequence[j].actualIndex != j)
                {
                    // Debug.LogError("Sequence is not correct at index " + j);
                    isCorrect = false;
                }
            }
            
            Debug.Log("Sequence iS " + (isCorrect ? "CORRECT!" : "INCORRECT!"));

            if (isCorrect)
            {     
                StartCoroutine(playAudioSequentially(finishSource, isCorrect));
            }
        }
    }
    
    IEnumerator playAudioSequentially(AudioSource adSource, bool isCorrect)
    {
        yield return new WaitForSeconds(1);

        //1.Loop through each AudioClip
        for (int i = 0; i < sequence.Count; i++)
        {
            //2.Assign current AudioClip to audiosource
            var play = btns[i];
        
            //3.Play Audio
            play.Play();
        
            //4.Wait for it to finish playing
            while (play.isClipPlaying)
            {
                yield return null;
            }
        
            //5. Go back to #2 and play the next audio in the adClips array
        
            if (isCorrect && i == sequence.Count - 2)
            {
                OnComplete?.Invoke();
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            if (indexPlaying >= 0 && indexPlaying < btns.Length)
            {
                if (!btns[indexPlaying].isClipPlaying)
                {
                    isPlaying = false;
                }
            }
            else
            {
                isPlaying = false;
            }
        }
    }

    public void InsertBtnHere(ProtoBtnClipDragUI btn, ProtoBtnClipPlay insertBefore)
    {
        var newIndex = insertBefore.Data.index;
        var oldItemIndex = btn.btn.Data.index;
        
        // move the actual data around in the sequence
        var item = sequence[oldItemIndex];
        sequence.RemoveAt(oldItemIndex);
        sequence.Insert(newIndex, item);

        // update the index of each item in the sequence
        for (int i = 0; i < sequence.Count; i++)
        {
            var s = sequence[i];
            s.index = i;
            sequence[i] = s;
        }
        
        // then populate the same butttons with the new order
        SetButtons();
        CheckComplete();
    }
}

[Serializable]
public struct GameLevelData
{
    public string songID;
    public float startingPoint;
    public List<GameClipData> clips;

    public AudioClip GetAudioClip()
    {
        var clip = Resources.Load<AudioClip>("songs/" + songID);
        return clip;
    }
}

[Serializable]
public struct GameClipData
{
    [HideInInspector]
    public float startingPoint;
    public float duration;
}