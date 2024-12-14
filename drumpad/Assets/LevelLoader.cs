using UnityEngine;
using System.IO;

public class LevelLoader : MonoBehaviour
{
    public string levelFilePath = "slimshady-easy-1";
    public GameLevelData LoadLevel()
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
