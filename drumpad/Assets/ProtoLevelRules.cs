using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoLevelRules : MonoBehaviour
{
    
}

[SerializeField]
public struct LevelRulesData
{
    public int moves;
    public enum eGameType
    {
        SongClips = 0,
        AlbumArt,        
    }
    public eGameType gameType;
}
