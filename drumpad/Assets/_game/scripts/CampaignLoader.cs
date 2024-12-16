using Unity.VisualScripting;
using UnityEngine;

public class CampaignLoader : MonoBehaviour
{
    [Header("Level Debug Testing")]
    public bool isDebug = false;

    private bool IsDebug
    {
        get 
        {
            // TODO some #if UNITY_ANDROID and stuff like that
            #if UNITY_EDITOR
            return isDebug;
            #else
            return false;
            #endif
        }
    }

    public string debugOverrideLevelName = "level-1"; // Name of the campaign JSON file without extension

    [Header("Campaign stuff")]
    public string campaignFileName = "campaign-1"; // Name of the campaign JSON file without extension

    private Campaign currentCampaign;
    private CampaignProgressManager progressManager;
    private CampaignProgressManager ProgressManager
    {
        get
        {
            progressManager = GetComponent<CampaignProgressManager>();
            if (progressManager == null)
            {
                progressManager = gameObject.AddComponent<CampaignProgressManager>();
            }
            return progressManager;
        }
    }

    public void LoadCampaign()
    {
        if (isDebug)
        {
            Debug.LogWarning("PLAYING IN DEBUG MODE - TESTING LEVEL " + debugOverrideLevelName + "! Go to CampaignLoader in scene and turn it off if not intentional!");
            return;  
        } 

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
        ProgressManager.LoadProgress(campaignFileName);
    }

    public GameLevelData StartNextLevel()
    {
        if (isDebug)
        {
            return LevelLoader.LoadLevel(debugOverrideLevelName);
        }

        int currentLevelIndex = ProgressManager.GetCurrentLevelIndex();
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
            return LevelLoader.LoadLevel(currentCampaign.levels[currentCampaign.levels.Count - 1]);
        }
    }

    public void CompleteCurrentLevel()
    {
        if (isDebug) return;

        int currentLevelIndex = ProgressManager.GetCurrentLevelIndex();
        if (currentLevelIndex < currentCampaign.levels.Count)
        {
            string completedLevel = currentCampaign.levels[currentLevelIndex];
            Debug.Log($"Complete Level {completedLevel}");
            ProgressManager.CompleteLevel(completedLevel);
        }
    }
}
