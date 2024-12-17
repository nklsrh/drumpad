using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class LevelEditorWindow : EditorWindow
{
    private string[] availableLevels; // List of available level file names
    private int selectedLevelIndex = 0; // Current selected level index
    private string currentLevelFileName = ""; // Selected level file name

    private GameLevelData currentLevelData;
    private string savePath = "Assets/Resources/Levels/";
    private AudioSource editorAudioSource; // AudioSource for playing clips
    private double stopTime = 0; // To track when to stop playback
    private string saveFileName = "";

    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
    }

    private void OnEnable()
    {
        if (editorAudioSource == null)
        {
            GameObject audioSourceObject = new GameObject("EditorAudioSource");
            audioSourceObject.hideFlags = HideFlags.HideAndDontSave;
            editorAudioSource = audioSourceObject.AddComponent<AudioSource>();
        }

        RefreshAvailableLevels();
    }

    private void OnDisable()
    {
        if (editorAudioSource != null)
        {
            DestroyImmediate(editorAudioSource.gameObject);
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Level Editor", EditorStyles.boldLabel);

        // Refresh Level List
        if (GUILayout.Button("Refresh Levels", GUILayout.Height(50)))
        {
            RefreshAvailableLevels();
        }

        GUILayout.BeginHorizontal();
        // Dropdown to select a level
        EditorGUILayout.LabelField("Select a Level:");
        if (availableLevels.Length > 0)
        {
            var i = selectedLevelIndex;
            selectedLevelIndex = EditorGUILayout.Popup(selectedLevelIndex, availableLevels);
            currentLevelFileName = availableLevels[selectedLevelIndex];
        }
        else
        {
            EditorGUILayout.HelpBox("No levels found in 'Assets/Resources/Levels'. Add JSON files to the folder.", MessageType.Warning);
        }

        // Load selected level
        if (GUILayout.Button("Load Level"))
        {
            LoadSelectedLevel();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(50);

        // Level Data Editing
        if (currentLevelData.clips != null)
        {
            GUIStyle SectionNameStyle = new GUIStyle();
            SectionNameStyle.fontSize = 30;
            SectionNameStyle.fontStyle = FontStyle.Bold;

            EditorGUILayout.LabelField("SONG SELECTED: ", currentLevelFileName, SectionNameStyle, GUILayout.Width(300), GUILayout.Height(50));

            // Level Rules Section
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Level Rules", EditorStyles.boldLabel);

            currentLevelData.moves = EditorGUILayout.IntField("Moves", currentLevelData.moves);
            currentLevelData.gameType = (GameLevelData.eGameType)EditorGUILayout.EnumPopup("Game Type", currentLevelData.gameType);
            currentLevelData.rewardCoins = EditorGUILayout.IntField("Reward Coins", currentLevelData.rewardCoins);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Level Layout", EditorStyles.boldLabel);

            currentLevelData.songID = EditorGUILayout.TextField("Song ID", currentLevelData.songID);
            currentLevelData.startingPoint = EditorGUILayout.FloatField("Starting Point", currentLevelData.startingPoint);

            EditorGUILayout.LabelField("Clips", EditorStyles.boldLabel);
            for (int i = 0; i < currentLevelData.clips.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                var c = currentLevelData.clips[i];
                c.startingPoint = (i>0? currentLevelData.clips[i - 1].startingPoint + currentLevelData.clips[i-1].duration : currentLevelData.startingPoint);
                EditorGUILayout.LabelField("Starting Point: " +  c.startingPoint + "s");
                c.duration = EditorGUILayout.FloatField("Duration", currentLevelData.clips[i].duration);
                c.isCorrectByDefault = EditorGUILayout.Toggle("Lock", c.isCorrectByDefault);
                currentLevelData.clips[i] = c;

                if (GUILayout.Button("Play Audio"))
                {
                    PlayClip(i);
                }
                
                if (GUILayout.Button("Remove"))
                {
                    currentLevelData.clips.RemoveAt(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Clip"))
            {
                currentLevelData.clips.Add(new GameClipData());
            }
        }

        GUILayout.Space(50);


        saveFileName = EditorGUILayout.TextField("Filename", saveFileName);

        // Save Level
        if (GUILayout.Button("Save Level"))
        {
            SaveLevel();
        }
    }

    private void RefreshAvailableLevels()
    {
        string levelsPath = Path.Combine(Application.dataPath, "Resources/Levels");
        if (Directory.Exists(levelsPath))
        {
            var files = Directory.GetFiles(levelsPath, "*.json");
            availableLevels = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                availableLevels[i] = Path.GetFileNameWithoutExtension(files[i]);
            }
        }
        else
        {
            availableLevels = new string[0];
        }

        // Reset selection if no levels exist
        if (availableLevels.Length == 0)
        {
            selectedLevelIndex = 0;
            currentLevelFileName = "";
        }
    }

    private void LoadSelectedLevel()
    {
        if (string.IsNullOrEmpty(currentLevelFileName))
        {
            Debug.LogError("No level selected to load!");
            return;
        }

        string filePath = Path.Combine(savePath, currentLevelFileName + ".json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            currentLevelData = JsonUtility.FromJson<GameLevelData>(json);
            Debug.Log($"Level '{currentLevelFileName}' loaded successfully.");

            saveFileName = currentLevelFileName;
        }
        else
        {
            Debug.LogError($"File not found: {filePath}");
        }
    }

    private void SaveLevel()
    {
        if (currentLevelData.IsEmpty())
        {
            Debug.LogError("No level data to save!");
            return;
        }

        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        string filePath = Path.Combine(savePath, saveFileName + ".json");
        string json = JsonUtility.ToJson(currentLevelData, true);
        File.WriteAllText(filePath, json);
        AssetDatabase.Refresh();
        Debug.Log($"Level '{saveFileName}' saved to '{filePath}'.");

        RefreshAvailableLevels();
    }


    private void PlayClip(int clipIndex)
    {
        if (string.IsNullOrEmpty(currentLevelData.songID))
        {
            Debug.LogError("Song ID is not set!");
            return;
        }

        AudioClip audioClip = Resources.Load<AudioClip>("songs/"+currentLevelData.songID);
        if (audioClip == null)
        {
            Debug.LogError($"Audio clip not found in Resources: {currentLevelData.songID}");
            return;
        }

        GameClipData clipData = currentLevelData.clips[clipIndex];
        float startPoint = clipData.startingPoint;
        float duration = clipData.duration;

        if (startPoint + duration > audioClip.length)
        {
            Debug.LogWarning("Clip duration exceeds audio clip length. Adjusting playback duration.");
            duration = audioClip.length - startPoint;
        }

        editorAudioSource.clip = audioClip;
        editorAudioSource.time = startPoint;
        editorAudioSource.Play();
        Debug.Log($"Playing clip {clipIndex}: Starting at {startPoint}, Duration {duration}");

        // Schedule the stop time
        stopTime = EditorApplication.timeSinceStartup + duration;

        // Subscribe to EditorApplication.update for stopping playback
        EditorApplication.update += StopPlaybackAfterDuration;
    }

    private void StopPlaybackAfterDuration()
    {
        // Check if the stop time has been reached
        if (EditorApplication.timeSinceStartup >= stopTime)
        {
            if (editorAudioSource.isPlaying)
            {
                editorAudioSource.Stop();
                Debug.Log("Playback stopped.");
            }

            // Unsubscribe from EditorApplication.update
            EditorApplication.update -= StopPlaybackAfterDuration;
        }
    }

}