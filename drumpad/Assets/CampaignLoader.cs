using UnityEngine;
using System.Collections.Generic;

public class CampaignLoader : MonoBehaviour
{
    public string campaignFileName = "Campaign1"; // Name of the campaign JSON file without extension
    private Campaign currentCampaign;

    public GameLevelData CurrentLevel;

    public void LoadCampaign(int level)
    {
        // Load campaign JSON from Resources
        TextAsset campaignJson = Resources.Load<TextAsset>($"Campaigns/{campaignFileName}");
        if (campaignJson == null)
        {
            Debug.LogError($"Campaign JSON file not found in Resources: {campaignFileName}");
            return;
        }

        // Parse the campaign JSON
        currentCampaign = JsonUtility.FromJson<Campaign>(campaignJson.text);
        Debug.Log($"Campaign Loaded: {currentCampaign.levels.Count} levels");

        // Load each level
        // foreach (var levelFileName in currentCampaign.levels)
        // {
        //     Debug.Log($"Loading level: {levelFileName}");
        //     LoadLevel(levelFileName);
        // }
        LoadLevel(currentCampaign.levels[level]);
    }

    private void LoadLevel(string levelFileName)
    {
        TextAsset levelJson = Resources.Load<TextAsset>($"Levels/{levelFileName}");
        if (levelJson == null)
        {
            Debug.LogError($"Level JSON file not found in Resources: {levelFileName}");
            return;
        }

        CurrentLevel = JsonUtility.FromJson<GameLevelData>(levelJson.text);
        Debug.Log($"Loaded level: {levelFileName} with songID {CurrentLevel.songID}");
    }
}
