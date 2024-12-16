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

    public int movesTaken;

    public event Action<ProtoAudioClipControl> OnStart;
    public event Action<ProtoAudioClipControl> OnMove;
    public static event Action<bool> OnComplete;

    public void StartGame(GameLevelData gameLevelData)
    {
        this.gameLevelData = gameLevelData;
        movesTaken = 0;

        StartCoroutine(WaitThenStart());
    }

    private IEnumerator WaitThenStart()
    {
        LoadLevelData(gameLevelData);

        yield return new WaitForSeconds(0.01f);
        
        var randomIndex = RandomiseClips();
        int loops = 1000;
        while (CheckRandomness(randomIndex) && loops > 0)
        {
            loops--;
            randomIndex = RandomiseClips();
        }

        sequence.Clear();

        for (int i = 0; i < randomIndex.Length; i++)
        {
            int randomIndexValue = randomIndex[i];
            AddClipToSequence(randomIndexValue);
        }

        SetLevelData();
        SetButtons();

        // just in case some fuckwit set up a broken level that automatically wins (1 clip or some shit)
        var isComplete = CheckCompleteAndFinish();

        if (!isComplete)
        {
            OnStart?.Invoke(this);

            yield return new WaitForSeconds(1.2f);

            PlaySound(0);
        }
    }

    private void SetLevelData()
    {
        for (int i = 0; i < btns.Length; i++)
        {
            if (sequence.Count > i)
            {
                btns[i].gameObject.SetActive(true);
                btns[i].SetLevelData(gameLevelData);
            }
            else
            {
                btns[i].gameObject.SetActive(false);
            }
        }
    }

    bool CheckRandomness(int[] randomIndex)
    {
        if (randomIndex.Length == gameLevelData.clips.Count)
        {
            bool isCorrect = true;
            for (int j = 0; j < randomIndex.Length; j++)
            {
                if (randomIndex[j] != j)
                {
                    isCorrect = false;
                }
            }

            return isCorrect;
        }
        return false;
    }

    int[] RandomiseClips()
    {
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
        return randomIndex;
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
            for (int i = 0; i < btns.Length; i++)
            {
                btns[i].Stop();
            }
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
            locked = shouldFreezeIfFirst && sequence.Count == 0,
            actualIndex = randomIndex,
            assignedTileImageIndex = UnityEngine.Random.Range(0,192)
        });
    }

    bool CheckComplete()
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
            

            return isCorrect;
        }
        return false;
    }
    
    IEnumerator playAudioSequentially(AudioSource adSource, bool isCorrect)
    {
        OnPlayTestSequence?.Invoke();

        yield return new WaitForSeconds(0.4f);

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
                OnComplete?.Invoke(true);
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

        if (newIndex == oldItemIndex) return;
        
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

        btns[newIndex].Drop();
        for (int i = newIndex + 1; i < sequence.Count; i++)
        {
            btns[i].SlideIn(0.05f * (i - newIndex + 1), i == newIndex ? -120 : -120);
        }

        bool isFinished = CheckCompleteAndFinish();
        if (!isFinished)
        {
            AddMove();
        }
    }

    private void AddMove()
    {
        movesTaken++;
        if (OnMove != null)
        {
            OnMove(this);
        }
        CheckMovesOver();
    }

    public void ReplenishMoves(int moves)
    {
        movesTaken -= moves;
        if (OnMove != null)
        {
            OnMove(this);
        }
    }

    private void CheckMovesOver()
    {
        if (movesTaken > gameLevelData.moves)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        if (OnComplete != null)
        {
            OnComplete.Invoke(false);
        }
    }

    bool CheckCompleteAndFinish()
    {
        var isComplete = CheckComplete();
        Debug.Log("Sequence iS " + (isComplete ? "CORRECT!" : "INCORRECT!"));

        if (isComplete)
        {
            StartCoroutine(playAudioSequentially(finishSource, true));
            return true;
        }
        return false;
    }
}