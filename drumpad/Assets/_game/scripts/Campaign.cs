using System;
using System.Collections.Generic;

[System.Serializable]
public struct Campaign
{
    public string id;
    public List<string> levels; // List of level file names

    internal bool IsEmpty()
    {
        return string.IsNullOrEmpty(id);
    }
}