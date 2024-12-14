using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class CampaignEditor : EditorWindow
{
    private Campaign currentCampaign;
    private string fileName = "NewCampaign"; // File name without extension
    private string[] levelFiles; // List of level files from Levels folder
    private int[] selectedIndices; // Tracks selected levels for dropdown

    private Vector2 scrollPos;

    [MenuItem("Tools/Campaign Editor")]
    public static void ShowWindow()
    {
        GetWindow<CampaignEditor>("Campaign Editor");
    }

    private void OnEnable()
    {
        LoadLevelFiles();
        InitializeSelectedIndices();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Campaign Editor", EditorStyles.boldLabel);

        // Display level files available in the Levels folder
        if (levelFiles == null || levelFiles.Length == 0)
        {
            EditorGUILayout.HelpBox("No level files found in the Levels folder. Add JSON files to 'Assets/Resources/Levels'.", MessageType.Warning);
            if (GUILayout.Button("Refresh Level List"))
            {
                LoadLevelFiles();
                InitializeSelectedIndices();
            }
            return;
        }

        EditorGUILayout.LabelField("Levels", EditorStyles.boldLabel);

        // Display levels in the campaign with dropdown for selection
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));
        for (int i = 0; i < currentCampaign.levels.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            // Dropdown to select a level
            selectedIndices[i] = GetDropdownIndex(currentCampaign.levels[i]);
            selectedIndices[i] = EditorGUILayout.Popup(selectedIndices[i], levelFiles);

            // Update the campaign level with the selected file
            currentCampaign.levels[i] = levelFiles[selectedIndices[i]];

            // Remove button
            if (GUILayout.Button("Remove", GUILayout.Width(70)))
            {
                currentCampaign.levels.RemoveAt(i);
                selectedIndices = RemoveIndexFromArray(selectedIndices, i);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        // Add new level
        if (GUILayout.Button("Add Level"))
        {
            currentCampaign.levels.Add(levelFiles[0]); // Default to the first level file
            selectedIndices = AddIndexToArray(selectedIndices, 0);
        }

        // Save and Load Campaign
        EditorGUILayout.Space();
        fileName = EditorGUILayout.TextField("File Name", fileName);

        if (GUILayout.Button("Save Campaign"))
        {
            SaveCampaign();
        }

        if (GUILayout.Button("Load Campaign"))
        {
            LoadCampaign();
            InitializeSelectedIndices();
        }

        if (GUILayout.Button("Refresh Level List"))
        {
            LoadLevelFiles();
            InitializeSelectedIndices();
        }
    }

    private void LoadLevelFiles()
    {
        string levelsPath = Path.Combine(Application.dataPath, "Resources/Levels");
        if (Directory.Exists(levelsPath))
        {
            var files = Directory.GetFiles(levelsPath, "*.json");
            levelFiles = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                levelFiles[i] = Path.GetFileNameWithoutExtension(files[i]);
            }
        }
        else
        {
            levelFiles = new string[0];
        }

        Debug.Log($"Found {levelFiles.Length} level files in Levels folder.");
    }

    private void InitializeSelectedIndices()
    {
        selectedIndices = new int[currentCampaign.levels.Count];
        for (int i = 0; i < currentCampaign.levels.Count; i++)
        {
            selectedIndices[i] = GetDropdownIndex(currentCampaign.levels[i]);
        }
    }

    private int GetDropdownIndex(string levelName)
    {
        for (int i = 0; i < levelFiles.Length; i++)
        {
            if (levelFiles[i] == levelName)
                return i;
        }
        return 0; // Default to the first option if not found
    }

    private int[] RemoveIndexFromArray(int[] array, int indexToRemove)
    {
        var list = new List<int>(array);
        list.RemoveAt(indexToRemove);
        return list.ToArray();
    }

    private int[] AddIndexToArray(int[] array, int valueToAdd)
    {
        var list = new List<int>(array) { valueToAdd };
        return list.ToArray();
    }

    private void SaveCampaign()
    {
        string savePath = Path.Combine("Assets/Resources/Campaigns", fileName + ".json");
        string folderPath = Path.GetDirectoryName(savePath);

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string json = JsonUtility.ToJson(currentCampaign, true);
        File.WriteAllText(savePath, json);
        AssetDatabase.Refresh();
        Debug.Log($"Campaign saved to {savePath}");
    }

    private void LoadCampaign()
    {
        string loadPath = Path.Combine("Assets/Resources/Campaigns", fileName + ".json");

        if (!File.Exists(loadPath))
        {
            Debug.LogError($"Campaign file not found: {loadPath}");
            return;
        }

        string json = File.ReadAllText(loadPath);
        currentCampaign = JsonUtility.FromJson<Campaign>(json);
        Debug.Log($"Campaign loaded from {loadPath}");
    }
}
