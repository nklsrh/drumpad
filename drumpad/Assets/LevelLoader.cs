using UnityEngine;

public class LevelLoader
{
    public static GameLevelData LoadLevel(string levelFilePath)
    {
        var text = Resources.Load<TextAsset>("Levels/" + levelFilePath);
        if (text != null)
        {
            var currentLevelData = JsonUtility.FromJson<GameLevelData>(text.text);
            Debug.Log("Level Loaded Successfully");
            return currentLevelData;
        }
        else
        {
            Debug.LogError("Level file not found: " + levelFilePath);
            return new GameLevelData();
        }
    }
}
