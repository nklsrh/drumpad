using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct GameLevelData
{
    public string songID;
    public float startingPoint;
    public List<GameClipData> clips;
    public int moves;
    public enum eGameType
    {
        SongClips = 0,
        AlbumArt,        
    }
    public eGameType gameType;

    public AudioClip GetAudioClip()
    {
        var clip = Resources.Load<AudioClip>("songs/" + songID);
        return clip;
    }

    internal bool IsEmpty()
    {
        return string.IsNullOrEmpty(songID);
    }
}

[Serializable]
public struct GameClipData
{
    [HideInInspector]
    public float startingPoint;
    public float duration;
}