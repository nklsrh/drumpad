using System.Collections.Generic;

[System.Serializable]
public class CampaignProgress
{
    public string campaignName; // Name of the campaign
    public int currentLevelIndex; // Index of the current level
    public List<string> completedLevels = new List<string>(); // List of completed level file names

    public CampaignProgress(string name)
    {
        campaignName = name;
        currentLevelIndex = 0;
        completedLevels = new List<string>();
    }

    internal string GetCurrentLevelIndexString()
    {
        return (int)(currentLevelIndex + 1)+"";
    }
}
