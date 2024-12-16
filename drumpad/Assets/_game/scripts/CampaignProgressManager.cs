using UnityEngine;
using System.Collections.Generic;

public class CampaignProgressManager : MonoBehaviour
{
    private CampaignProgress currentProgress;

    // Load progress for a specific campaign
    public void LoadProgress(string campaignName)
    {
        if (PlayerPrefs.HasKey(campaignName))
        {
            string json = PlayerPrefs.GetString(campaignName);
            currentProgress = JsonUtility.FromJson<CampaignProgress>(json);
            Debug.Log($"Loaded progress for campaign: {campaignName} with {currentProgress.currentLevelIndex} levels completed");
        }
        else
        {
            // Initialize new progress if none exists
            currentProgress = new CampaignProgress(campaignName);
            Debug.Log($"No progress found for campaign: {campaignName}. Starting fresh.");
        }
    }

    // Save current progress
    public void SaveProgress()
    {
        if (currentProgress != null)
        {
            string json = JsonUtility.ToJson(currentProgress);
            PlayerPrefs.SetString(currentProgress.campaignName, json);
            PlayerPrefs.Save();
            Debug.Log($"Progress saved for campaign: {currentProgress.campaignName}");
        }
    }

    // Get the current level index
    public int GetCurrentLevelIndex()
    {
        return currentProgress?.currentLevelIndex ?? 0;
    }

    // Mark a level as completed and move to the next level
    public void CompleteLevel(string levelName)
    {
        if (currentProgress != null)
        {
            if (currentProgress.completedLevels == null)
            {
                currentProgress.completedLevels = new List<string>();
            }
            currentProgress.completedLevels.Add(levelName);
            if (currentProgress.currentLevelIndex < currentProgress.completedLevels.Count)
            {
                currentProgress.currentLevelIndex++;
            }
            SaveProgress();
            Debug.Log($"Level {levelName} completed. Moving to level index {currentProgress.currentLevelIndex}");
        }
    }

    // Check if a level is completed
    public bool IsLevelCompleted(string levelName)
    {
        return currentProgress != null && currentProgress.completedLevels.Contains(levelName);
    }
}
