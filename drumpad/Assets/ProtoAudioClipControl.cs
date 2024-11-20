using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProtoAudioClipControl : MonoBehaviour
{
    public AudioClip[] clips;
    public ProtoBtnClipPlay[] btns;

    public AudioClip finishClip;
    public AudioSource finishSource;

    private int indexPlaying = -1;
    private bool isPlaying;
    
    public static event Action<StructAddClipSequence> OnAddClipSequence;
    public static event Action OnPlayTestSequence;
    private List<StructBtnData> sequence = new List<StructBtnData>();

    public GameObject panelComplete;

    // Start is called before the first frame update
    void Start()
    {
        panelComplete.SetActive(false);
    }

    public void StartGame()
    {
        StartCoroutine(WaitThenStart());
    }

    private IEnumerator WaitThenStart()
    {
        yield return new WaitForSeconds(0.5f);
        
        // make new int that randomises values in clips
        int[] randomIndex = new int[clips.Length];
        for (int i = 0; i < randomIndex.Length; i++)
        {
            randomIndex[i] = i;
        }

        for (int i = 0; i < randomIndex.Length; i++)
        {
            int temp = randomIndex[i];
            int randomIndexValue = UnityEngine.Random.Range(i, randomIndex.Length);
            randomIndex[i] = randomIndex[randomIndexValue];
            randomIndex[randomIndexValue] = temp;
        }
        
        for (int i = 0; i < btns.Length; i++)
        {
            int randomIndexValue = randomIndex[i];
            btns[randomIndexValue].gameObject.SetActive(true);
            btns[randomIndexValue].SetData(new StructBtnData()
            {
                index = randomIndexValue,
                actualIndex = i,
                clip = clips[i],
            });
            btns[randomIndexValue].SetAction(PlaySound);
            if (i == 0)
            {
                btns[randomIndexValue].Play();
                PlaySound(randomIndexValue);
                AddClipToSequence(randomIndexValue);
                // btns[randomIndexValue].gameObject.SetActive(false);
            }
        }
    }

    private void PlaySound(int index)
    {
        if (isPlaying)
        {
            if (index == indexPlaying)
            {
                AddClipToSequence(index);
            }
            return;
        }

        btns[index].Play();
        indexPlaying = index;
        isPlaying = true;
    }

    private void AddClipToSequence(int index)
    {
        btns[index].SetEnabled(false);
        OnAddClipSequence(new StructAddClipSequence()
        {
            index = index,
            data = btns[index].Data
        });
        sequence.Add(btns[index].Data);

        CheckComplete();
    }

    void CheckComplete()
    {
        if (sequence.Count == clips.Length)
        {
            bool isCorrect = true;
            // check each element of btns has an Data value that is increasing
            for (int j = 0; j < clips.Length; j++)
            {
                if (sequence[j].actualIndex != j)
                {
                    Debug.LogError("Sequence is not correct at index " + j);
                    isCorrect = false;
                }
            }
            
            Debug.Log("Sequence iS " + (isCorrect ? "CORRECT!" : "INCORRECT!"));

            List<AudioClip> playClips = new List<AudioClip>();
            playClips.AddRange(sequence.Select(r=>r.clip));
            if (isCorrect)
            {
                playClips.Add(finishClip);
            }
            
            StartCoroutine(playAudioSequentially(playClips, finishSource, isCorrect));
        }
    }
    
    IEnumerator playAudioSequentially(List<AudioClip> adClips, AudioSource adSource, bool isCorrect)
    {
        yield return new WaitForSeconds(1);

        //1.Loop through each AudioClip
        for (int i = 0; i < adClips.Count; i++)
        {
            //2.Assign current AudioClip to audiosource
            adSource.clip = adClips[i];

            //3.Play Audio
            adSource.Play();

            //4.Wait for it to finish playing
            while (adSource.isPlaying)
            {
                yield return null;
            }

            //5. Go back to #2 and play the next audio in the adClips array
        
            if (isCorrect && i == adClips.Count - 2)
            {
                panelComplete.SetActive(true);
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
}

public struct StructBtnData
{
    public int index;
    public int actualIndex;
    public AudioClip clip;
}

public struct StructAddClipSequence
{
    public int index;
    public StructBtnData data;
}
