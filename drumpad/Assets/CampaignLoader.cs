using UnityEngine;

public class CampaignLoader : MonoBehaviour
{
    public string campaignFileName = "campaign-1"; // Name of the campaign JSON file without extension

    private Campaign currentCampaign;
    private CampaignProgressManager progressManager;

    private void Awake()
    {
        progressManager = GetComponent<CampaignProgressManager>();
        if (progressManager == null)
        {
            progressManager = gameObject.AddComponent<CampaignProgressManager>();
        }
    }

    public void LoadCampaign()
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

        // Load player progress for this campaign
        progressManager.LoadProgress(campaignFileName);
    }

    public GameLevelData StartNextLevel()
    {
        int currentLevelIndex = progressManager.GetCurrentLevelIndex();
        if (currentLevelIndex < currentCampaign.levels.Count)
        {
            string nextLevel = currentCampaign.levels[currentLevelIndex];
            Debug.Log($"Starting level: {nextLevel}");
            // Logic to load the level

            return LevelLoader.LoadLevel(nextLevel);            
        }
        else
        {
            Debug.Log("All levels completed!");
            return new GameLevelData();
        }
    }

    public void CompleteCurrentLevel()
    {
        int currentLevelIndex = progressManager.GetCurrentLevelIndex();
        if (currentLevelIndex < currentCampaign.levels.Count)
        {
            string completedLevel = currentCampaign.levels[currentLevelIndex];
            Debug.Log($"Complete Level {completedLevel}");
            progressManager.CompleteLevel(completedLevel);
        }
    }
}
